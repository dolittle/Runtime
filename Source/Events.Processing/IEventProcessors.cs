// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that knows about Event Processors.
    /// </summary>
    public interface IEventProcessors
    {
        /// <summary>
        /// Registers an Event Processor.
        /// </summary>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="streamProcessorRegistrations">The <see cref="StreamProcessorRegistrations" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="EventProcessorRegistrationResult" />.</returns>
        Task<EventProcessorRegistrationResult> Register(StreamId sourceStreamId, IEventProcessor eventProcessor, StreamProcessorRegistrations streamProcessorRegistrations, CancellationToken cancellationToken);
    }
}
