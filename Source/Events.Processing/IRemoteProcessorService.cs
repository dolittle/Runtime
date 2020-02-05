// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a service which can handle an event.
    /// </summary>
    public interface IRemoteProcessorService
    {
        /// <summary>
        /// Process an event.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of processing an event.</returns>
        Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId);
    }
}