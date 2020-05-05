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
    [BsonDiscriminator(RootClass = true, Required = true)]
    [BsonKnownTypes(typeof(FilterDefinition), typeof(PublicFilterDefinition))]
    public abstract class AbstractFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFilterDefinition"/> class.
        /// </summary>
        /// <param name="filterId">The filter id.</param>
        /// <param name="sourceStream">The source stream.</param>
        protected AbstractFilterDefinition(Guid filterId, Guid sourceStream)
        {
            FilterId = filterId;
            SourceStream = sourceStream;
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
        /// Converts the stored filter into the runtime <see cref="IFilterDefinition" /> that it represents.
        /// </summary>
        /// <returns>The runtime <see cref="IFilterDefinition" />.</returns>
        public abstract IFilterDefinition AsRuntimeRepresentation();
    }
}
