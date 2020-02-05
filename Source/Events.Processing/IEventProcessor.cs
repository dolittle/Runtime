// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that processes an event.
    /// </summary>
    public interface IEventProcessor
    {
        /// <summary>
        /// Gets the identifier for the <see cref="IEventProcessor"/>.
        /// </summary>
        EventProcessorId Identifier { get; }

        /// <summary>
        /// Processes an <see cref="CommittedEvent" /> for a <see cref="PartitionId"> partition </see>.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <returns><see cref="IProcessingResult" />.</returns>
        Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId);
    }
}