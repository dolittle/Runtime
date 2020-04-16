// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a hub for <see cref="StreamProcessor" />.
    /// </summary>
    public interface IStreamProcessors
    {
        /// <summary>
        /// Gets all registered stream processors.
        /// </summary>
        /// <returns>The<see cref="IEnumerable{StreamProcessor}" >stream processors</see>.</returns>
        IEnumerable<StreamProcessor> Processors { get; }

        /// <summary>
        /// Registers and starts a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="eventsFromStreamsFetcher">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="StreamProcessorRegistrationResult"/>.</returns>
        StreamProcessorRegistrationResult Register(IEventProcessor eventProcessor, IFetchEventsFromStreams eventsFromStreamsFetcher, StreamId sourceStreamId, CancellationToken cancellationToken);

        /// <summary>
        /// Unregister a <see cref="IEventProcessor"/> from stream processing.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" /> of the event processor.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        void Unregister(ScopeId scopeId, EventProcessorId eventProcessorId, StreamId sourceStreamId);
    }
}