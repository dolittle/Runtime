// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters
{
    /// <summary>
    /// Represents a persisted <see cref="Store.Streams.Filters.IFilterDefinition" />.
    /// </summary>
    public abstract class AbstractFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFilterDefinition"/> class.
        /// </summary>
        /// <param name="filterId">The filter id.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="partitioned">Whether the filter is partitioned.</param>
        /// <param name="isPublic">Whether the filter is public.</param>
        protected AbstractFilterDefinition(Guid filterId, Guid sourceStream, bool partitioned, bool isPublic)
        {
            FilterId = filterId;
            SourceStream = sourceStream;
            Partitioned = partitioned;
            Public = isPublic;
        }

        /// <summary>
        /// Gets or sets the filter id.
        /// </summary>
        [BsonId]
        public Guid FilterId { get; set; }

        /// <summary>
        /// Gets or sets the source stream.
        /// </summary>
        public Guid SourceStream { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the filter is partitioned or not.
        /// </summary>
        public bool Partitioned { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the filter defines a public stream definition.
        /// </summary>
        public bool Public { get; set; }

        /// <summary>
        /// Converts the stored filter into the runtime <see cref="IFilterDefinition" /> that it represents.
        /// </summary>
        /// <returns>The runtime <see cref="IFilterDefinition" />.</returns>
        public abstract IFilterDefinition AsRuntimeRepresentation();
    }
}
