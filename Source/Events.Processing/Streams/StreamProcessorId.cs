// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a unique key for a <see cref="StreamProcessor" />.
    /// </summary>
    public class StreamProcessorId : Value<StreamProcessorId>, IStreamProcessorId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorId"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId"><see cref="EventProcessorId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId"/>.</param>
        public StreamProcessorId(ScopeId scopeId, EventProcessorId eventProcessorId, StreamId sourceStreamId)
        {
            ScopeId = scopeId;
            EventProcessorId = eventProcessorId;
            SourceStreamId = sourceStreamId;
        }

        /// <inheritdoc/>
        public ScopeId ScopeId { get; }

        /// <summary>
        /// Gets the <see cref="EventProcessorId" />.
        /// </summary>
        public EventProcessorId EventProcessorId { get; }

        /// <summary>
        /// Gets the <see cref="StreamId" />.
        /// </summary>
        public StreamId SourceStreamId { get; }

        /// <inheritdoc />
        public override string ToString() => $"Scope: {ScopeId} Event Processor Id: {EventProcessorId} Source Stream: {SourceStreamId}";
    }
}