// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Store;

using MongoDB.Bson;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition;

/// <summary>
/// Represents an implementation of <see cref="IConvertProjectionDefinition" />.
/// </summary>
[Singleton]
public class ConvertProjectionDefinition : IConvertProjectionDefinition
{
    public Store.Definition.ProjectionDefinition ToRuntime(
        ProjectionId projection,
        ScopeId scope,
        IEnumerable<ProjectionEventSelector> eventSelectors,
        Store.State.ProjectionState initialState,
        ProjectionCopies copies)
        => new(
            projection,
            scope,
            eventSelectors.Select(_ => new Store.Definition.ProjectionEventSelector(
                _.EventType,
                _.EventKeySelectorType,
                _.EventKeySelectorExpression,
                _.StaticKey ?? "",
                _.OccurredFormat ?? "")),
            initialState,
            ToRuntimeCopies(copies));

    static Store.Definition.Copies.ProjectionCopySpecification ToRuntimeCopies(ProjectionCopies specification)
        => specification == null
            ? new(CopyToMongoDBSpecification.Default)
            : new(ToRuntimeCopyToMongoDB(specification.MongoDB));

    static Store.Definition.Copies.MongoDB.CopyToMongoDBSpecification ToRuntimeCopyToMongoDB(ProjectionCopyToMongoDB specification)
        => specification == null
            ? CopyToMongoDBSpecification.Default
            : new(
                specification.ShouldCopyToMongoDB,
                specification.CollectionName,
                ToRuntimeCopyToMongoDBPropertyConversion(specification.Conversions));
    
    static Store.Definition.Copies.MongoDB.PropertyConversion[] ToRuntimeCopyToMongoDBPropertyConversion(IEnumerable<ProjectionCopyToMongoDBPropertyConversion> conversions)
        => conversions.Select(conversion => new Store.Definition.Copies.MongoDB.PropertyConversion(
            conversion.Property,
            conversion.ConversionType,
            conversion.ShouldRename,
            conversion.RenameTo,
            ToRuntimeCopyToMongoDBPropertyConversion(conversion.Children)
        )).ToArray();
    
    public ProjectionDefinition ToStored(Store.Definition.ProjectionDefinition definition)
        => new()
        {
            Projection = definition.Projection,
            InitialStateRaw = definition.InitialState,
            InitialState = BsonDocument.Parse(definition.InitialState),
            EventSelectors = definition.Events.Select(_ => new ProjectionEventSelector
            {
                EventKeySelectorType = _.KeySelectorType,
                EventKeySelectorExpression = _.KeySelectorExpression,
                EventType = _.EventType,
                StaticKey = _.StaticKey,
                OccurredFormat = _.OccurredFormat
            }).ToArray(),
            Copies = ToStoredCopies(definition.Copies),
        };

    static ProjectionCopies ToStoredCopies(Store.Definition.Copies.ProjectionCopySpecification specification)
        => new()
        {
            MongoDB = ToStoredCopyToMongoDB(specification.MongoDB),
        };

    static ProjectionCopyToMongoDB ToStoredCopyToMongoDB(Store.Definition.Copies.MongoDB.CopyToMongoDBSpecification specification)
        => new()
        {
            ShouldCopyToMongoDB = specification.ShouldCopyToMongoDB,
            CollectionName = specification.Collection,
            Conversions = ToStoredCopyToMongoDBPropertyConversion(specification.Conversions),
        };

    static IEnumerable<ProjectionCopyToMongoDBPropertyConversion> ToStoredCopyToMongoDBPropertyConversion(Store.Definition.Copies.MongoDB.PropertyConversion[] conversions)
        => conversions.Select(conversion => new ProjectionCopyToMongoDBPropertyConversion()
        {
            Property = conversion.Property,
            ConversionType = conversion.Conversion,
            ShouldRename = conversion.ShouldRename,
            RenameTo = conversion.RenameTo,
            Children = ToStoredCopyToMongoDBPropertyConversion(conversion.Children),
        });
}
