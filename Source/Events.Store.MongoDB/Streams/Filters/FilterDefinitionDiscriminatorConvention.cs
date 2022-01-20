// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;

/// <summary>
/// Represents our own custom <see cref="IDiscriminatorConvention"/> used to deal with our MongoDB representation of
/// <see cref="RemoteFilterDefinition"/>.
/// </summary>
public class FilterDefinitionDiscriminatorConvention : IDiscriminatorConvention
{
    const string Type = "Type";

    /// <summary>
    /// Represents the differnet kinds of FilterDefinitions we have.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Represents filters that get filtered in the head.
        /// </summary>
        Remote,

        /// <summary>
        /// Represents filters that get filtered in the runtime depending on a given declaration.
        /// </summary>
        EventTypeId
    }

    /// <inheritdoc/>
    public string ElementName => Type;

    /// <inheritdoc/>
    public BsonValue GetDiscriminator(Type nominalType, Type actualType)
    {
        if (actualType == typeof(RemoteFilterDefinition))
        {
            return nameof(FilterType.Remote);
        }

        if (actualType == typeof(TypePartitionFilterDefinition))
        {
            return nameof(FilterType.EventTypeId);
        }

        throw new UnsupportedTypeForFilterDefinitionDiscriminatorConvention(actualType);
    }

    /// <inheritdoc/>
    public Type GetActualType(IBsonReader bsonReader, Type nominalType)
    {
        ThrowIfNominalTypeIsIncorrect(nominalType);
        var bookmark = bsonReader.GetBookmark();
        bsonReader.ReadStartDocument();
        Guid id = default;

        while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
        {
            switch (bsonReader.ReadName())
            {
                case Type:
                    var filterType = Enum.Parse<FilterType>(bsonReader.ReadString());
                    bsonReader.ReturnToBookmark(bookmark);
                    return filterType switch
                    {
                        FilterType.EventTypeId => typeof(TypePartitionFilterDefinition),
                        FilterType.Remote => typeof(RemoteFilterDefinition),
                        _ => throw new UnsupportedFilterTypeEnumValue(filterType, id)
                    };
                case "_id":
                    id = bsonReader.ReadBinaryData().ToGuid(GuidRepresentation.Standard);
                    break;
                default:
                    bsonReader.SkipValue();
                    break;
            }
        }

        bsonReader.ReturnToBookmark(bookmark);
        throw new AbstractFilterDefinitionDocumentIsMissingTypeField(id);
    }

    void ThrowIfNominalTypeIsIncorrect(Type nominalType)
    {
        if (!typeof(AbstractFilterDefinition).IsAssignableFrom(nominalType))
            throw new UnsupportedTypeForFilterDefinitionDiscriminatorConvention(nominalType);
    }
}