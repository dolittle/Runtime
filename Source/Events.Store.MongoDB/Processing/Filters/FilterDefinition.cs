// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Filters;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents a persisted <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    [BsonDiscriminator(RootClass = true, Required = true)]
    [BsonKnownTypes(typeof(TypePartitionFilterDefinition))]
    public class FilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterDefinition"/> class.
        /// </summary>
        /// <param name="filterId">The filter id.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="partitioned">Whether the filter is partitioned or not.</param>
        public FilterDefinition(Guid filterId, Guid sourceStream, bool partitioned)
        {
            FilterId = filterId;
            SourceStream = sourceStream;
            Partitioned = partitioned;
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
        /// Converts the stored filter into the runtime <see cref="IFilterDefinition" /> that it represents.
        /// </summary>
        /// <returns>The runtime <see cref="IFilterDefinition" />.</returns>
        public virtual IFilterDefinition AsRuntimeRepresentation() => new RemoteFilterDefinition(SourceStream, FilterId, Partitioned);
    }
}