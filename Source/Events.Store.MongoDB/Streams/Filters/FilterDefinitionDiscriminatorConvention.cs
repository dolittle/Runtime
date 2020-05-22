// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters
{
    /// <summary>
    /// Represents our own custom <see cref="IDiscriminatorConvention"/> used to deal with our MongoDB representation of
    /// <see cref="FilterDefinition"/>.
    /// </summary>
    public class FilterDefinitionDiscriminatorConvention : IDiscriminatorConvention
    {
        const string Public = "Public";
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
            if (actualType == typeof(FilterDefinition) || actualType == typeof(PublicFilterDefinition))
            {
                return nameof(FilterType.Remote);
            }
            else if (actualType == typeof(TypePartitionFilterDefinition))
            {
                return nameof(FilterType.EventTypeId);
            }
            else
            {
                throw new UnsupportedTypeForFilterDefinitionDiscriminatorConvention(actualType);
            }
        }

        /// <inheritdoc/>
        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            ThrowIfNominalTypeIsIncorrect(nominalType);
            var bookmark = bsonReader.GetBookmark();
            bsonReader.ReadStartDocument();
            Guid id = default;
            FilterType filterType = default;
            bool isFilterSet = false;
            bool publicValue = default;
            bool isPublicSet = false;

            while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
            {
                switch (bsonReader.ReadName())
                {
                    case Type:
                        filterType = Enum.Parse<FilterType>(bsonReader.ReadString());
                        isFilterSet = true;
                        break;
                    case Public:
                        publicValue = bsonReader.ReadBoolean();
                        isPublicSet = true;
                        break;
                    case "_id":
                        id = (Guid)bsonReader.ReadBinaryData();
                        break;
                    default:
                        bsonReader.SkipValue();
                        break;
                }
            }

            bsonReader.ReturnToBookmark(bookmark);
            ThrowIfFieldsNotFound(isFilterSet, isPublicSet, id);

            return filterType switch
            {
                FilterType.EventTypeId => typeof(TypePartitionFilterDefinition),
                FilterType.Remote => publicValue ? typeof(PublicFilterDefinition) : typeof(FilterDefinition),
                _ => throw new UnsupportedFilterTypeEnumValue(filterType, id)
            };
        }

        void ThrowIfFieldsNotFound(bool isFilterSet, bool isPublicSet, Guid id)
        {
            if (!isFilterSet)
            {
                throw new AbstractFilterDefinitionDocumentIsMissingAField(id, Type);
            }

            if (!isPublicSet)
            {
                throw new AbstractFilterDefinitionDocumentIsMissingAField(id, Public);
            }
        }

        void ThrowIfNominalTypeIsIncorrect(Type nominalType)
        {
            if (!typeof(AbstractFilterDefinition).IsAssignableFrom(nominalType))
                throw new UnsupportedTypeForFilterDefinitionDiscriminatorConvention(nominalType);
        }
    }
}
