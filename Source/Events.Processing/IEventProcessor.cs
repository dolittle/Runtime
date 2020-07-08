// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that processes an event.
    /// </summary>
    public interface IEventProcessor
    {
        /// <summary>
        /// Gets the <see cref="Scope" />.
        /// </summary>
        ScopeId Scope { get; }

        /// <summary>
        /// Gets the identifier for the <see cref="IEventProcessor"/>.
        /// </summary>
        EventProcessorId Identifier { get; }

        /// <summary>
        /// /// Processes an <see cref="CommittedEvent" /> for a <see cref="PartitionId"> partition </see>.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns><see cref="IProcessingResult" />.</returns>
        Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken);

        /// <summary>
        /// Processes an <see cref="CommittedEvent" /> for a <see cref="PartitionId"> partition </see>.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="failureReason">The reason the processor was failing.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns><see cref="IProcessingResult" />.</returns>
        Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken);
    }
}