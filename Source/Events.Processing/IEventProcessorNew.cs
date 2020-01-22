// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that processes an event.
    /// </summary>
    public interface IEventProcessorNew // TODO: Change name to IEventProcessor and remove IEventProcessing from Source/Events/Processing.
    {
        /// <summary>
        /// Gets the identifier for the <see cref="IEventProcessorNew"/>.
        /// </summary>
        EventProcessorId Identifier { get; }

        /// <summary>
        /// Processes an <see cref="CommittedEventEnvelope">event</see>.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns><see cref="IProcessingResult" />.</returns>
        Task<IProcessingResult> Process(CommittedEventEnvelope @event);
    }
}