// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents the unique identifier for a <see cref="StreamProcessor" />.
    /// </summary>
    public class StreamProcessorId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorId"/> class.
        /// </summary>
        /// <param name="scopeId">The scope id.</param>
        /// <param name="eventProcessorId">The event processor id.</param>
        /// <param name="sourceStreamId">The source stream id.</param>
        public StreamProcessorId(ScopeId scopeId, Guid eventProcessorId, Guid sourceStreamId)
        {
            ScopeId = scopeId;
            EventProcessorId = eventProcessorId;
            SourceStreamId = sourceStreamId;
        }

        /// <summary>
        /// Gets or sets the scope id.
        /// </summary>
        public Guid ScopeId { get; set; }

        /// <summary>
        /// Gets or sets the event processor id.
        /// </summary>
        public Guid EventProcessorId { get; set; }

        /// <summary>
        /// Gets or sets the source stream id.
        /// </summary>
        public Guid SourceStreamId { get; set; }
    }
}