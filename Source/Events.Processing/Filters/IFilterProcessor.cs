// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Defines an <see cref="IEventProcessor" /> that filters a <see cref="CommittedEvent" />.
    /// </summary>
    /// <typeparam name="TDefinition">The <see cref="IFilterDefinition" />.</typeparam>
    public interface IFilterProcessor<TDefinition> : IEventProcessor
        where TDefinition : IFilterDefinition
    {
        /// <summary>
        /// Gets the <see cref="IFilterDefinition" /> for this filter.
        /// </summary>
        TDefinition Definition { get; }

        /// <summary>
        /// Filters the event.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="cancellationToken"> The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of filtering an event.</returns>
        Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, CancellationToken cancellationToken);

        /// <summary>
        /// Filters the event.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="failureReason">The reason the processor was failing.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="cancellationToken"> The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of filtering an event.</returns>
        Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, string failureReason, uint retryCount, CancellationToken cancellationToken);
    }
}