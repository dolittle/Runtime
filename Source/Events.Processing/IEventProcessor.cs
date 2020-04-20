// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using grpc = contracts::Dolittle.Runtime.Events.Processing;

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

        #nullable enable
        /// <summary>
        /// Processes an <see cref="CommittedEvent" /> for a <see cref="PartitionId"> partition </see>.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="retryProcessingState"><see cref="grpc.RetryProcessingState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns><see cref="IProcessingResult" />.</returns>
        Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, grpc.RetryProcessingState? retryProcessingState, CancellationToken cancellationToken);
    }
}