// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Protobuf;
using Artifact = Dolittle.Artifacts.Contracts.Artifact;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents an implementation of <see cref="IConvertProjectionDefinitions"/>.
/// </summary>
public class ConvertProjectionDefinitions : IConvertProjectionDefinitions
{
    /// <inheritdoc />
    public IEnumerable<ProjectionEventSelector> ToRuntimeEventSelectors(IEnumerable<Contracts.ProjectionEventSelector> selectors)
        => selectors.Select(eventSelector => eventSelector.SelectorCase switch
        {
            Contracts.ProjectionEventSelector.SelectorOneofCase.EventSourceKeySelector => ProjectionEventSelector.EventSourceId(eventSelector.EventType.Id.ToGuid()),
            Contracts.ProjectionEventSelector.SelectorOneofCase.PartitionKeySelector => ProjectionEventSelector.PartitionId(eventSelector.EventType.Id.ToGuid()),
            Contracts.ProjectionEventSelector.SelectorOneofCase.EventPropertyKeySelector => ProjectionEventSelector.EventProperty(eventSelector.EventType.Id.ToGuid(), eventSelector.EventPropertyKeySelector.PropertyName),
            _ => throw new InvalidProjectionEventSelector(eventSelector.SelectorCase)
        });

    /// <inheritdoc />
    public IEnumerable<Contracts.ProjectionEventSelector> ToContractsEventSelectors(IEnumerable<ProjectionEventSelector> selectors)
        => selectors.Select(eventSelector =>
        {
            var converted = new Contracts.ProjectionEventSelector
            {
                EventType = new Artifact
                {
                    Id = eventSelector.EventType.ToProtobuf(),
                    Generation = ArtifactGeneration.First,
                }
            };

            switch (eventSelector.KeySelectorType)
            {
                case ProjectEventKeySelectorType.EventSourceId:
                    converted.EventSourceKeySelector = new Contracts.EventSourceIdKeySelector();
                    break;
                case ProjectEventKeySelectorType.PartitionId:
                    converted.PartitionKeySelector = new Contracts.PartitionIdKeySelector();
                    break;
                case ProjectEventKeySelectorType.Property:
                    converted.EventPropertyKeySelector = new Contracts.EventPropertyKeySelector { PropertyName = eventSelector.KeySelectorExpression };
                    break;
            }
            
            return converted;
        });

    /// <inheritdoc />
    public ProjectionCopySpecification ToRuntimeCopySpecification(Contracts.ProjectionCopies copies)
    {
        var mongoDB = CopyToMongoDBSpecification.Default;
        if (copies?.MongoDB is { } copyToMongoDb)
        {
            mongoDB = new CopyToMongoDBSpecification(true, copyToMongoDb.Collection, ToRuntimePropertyConversions(copyToMongoDb.Conversions));
        }

        return new ProjectionCopySpecification(mongoDB);
    }

    /// <inheritdoc />
    public Contracts.ProjectionCopies ToContractsCopySpecification(ProjectionCopySpecification copies)
    {
        var converted = new Contracts.ProjectionCopies();

        if (copies.MongoDB.ShouldCopyToMongoDB)
        {
            converted.MongoDB = new Contracts.ProjectionCopyToMongoDB
            {
                Collection = copies.MongoDB.Collection,
            };
            converted.MongoDB.Conversions.AddRange(ToContractsPropertyConversions(copies.MongoDB.Conversions));
        }

        return converted;
    }

    static PropertyConversion[] ToRuntimePropertyConversions(IEnumerable<Contracts.ProjectionCopyToMongoDB.Types.PropertyConversion> conversions)
        => conversions.Select(conversion =>
            new PropertyConversion(
                conversion.PropertyName,
                conversion.ConvertTo switch
                {
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.None => ConversionBSONType.None,
                    
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsDate => ConversionBSONType.DateAsDate,
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsArray => ConversionBSONType.DateAsArray,
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsDocument => ConversionBSONType.DateAsDocument,
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsString => ConversionBSONType.DateAsString,
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsInt64 => ConversionBSONType.DateAsInt64,
                    
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasStandardBinary => ConversionBSONType.GuidAsStandardBinary,
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasCsharpLegacyBinary => ConversionBSONType.GuidAsCsharpLegacyBinary,
                    Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasString => ConversionBSONType.GuidAsString,
                    
                    _ => throw new InvalidMongoDBFieldConversion(conversion.PropertyName, conversion.ConvertTo),
                },
                conversion.RenameTo != default,
                conversion.RenameTo ?? "",
                ToRuntimePropertyConversions(conversion.Children))
            ).ToArray();

    static IEnumerable<Contracts.ProjectionCopyToMongoDB.Types.PropertyConversion> ToContractsPropertyConversions(PropertyConversion[] conversions)
        => conversions.Select(conversion =>
        {
            var converted = new Contracts.ProjectionCopyToMongoDB.Types.PropertyConversion
            {
                PropertyName = conversion.Property,
                ConvertTo = conversion.Conversion switch
                {
                    ConversionBSONType.None => Contracts.ProjectionCopyToMongoDB.Types.BSONType.None,
                    
                    ConversionBSONType.DateAsDate => Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsDate,
                    ConversionBSONType.DateAsArray => Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsArray,
                    ConversionBSONType.DateAsDocument => Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsDocument,
                    ConversionBSONType.DateAsString => Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsString,
                    ConversionBSONType.DateAsInt64 => Contracts.ProjectionCopyToMongoDB.Types.BSONType.DateAsInt64,
                    
                    ConversionBSONType.GuidAsStandardBinary => Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasStandardBinary,
                    ConversionBSONType.GuidAsCsharpLegacyBinary => Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasCsharpLegacyBinary,
                    ConversionBSONType.GuidAsString => Contracts.ProjectionCopyToMongoDB.Types.BSONType.GuidasString,
                },
                RenameTo = conversion.ShouldRename ? conversion.RenameTo : null,
            };
            converted.Children.AddRange(ToContractsPropertyConversions(conversion.Children));
            return converted;
        });
}
