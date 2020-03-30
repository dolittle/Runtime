// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines how we get and set the <see cref="StreamProcessorState"/>for <see cref="StreamProcessor" >stream processors</see>.
    /// </summary>
    public interface IStreamProcessorStateRepository
    {
        /// <summary>
        /// Gets the <see cref="StreamProcessorState" /> for this <see cref="StreamProcessor" /> or creates and adds a new one.
        /// </summary>
        /// <param name="streamProcessorId">The unique<see cref="StreamProcessorId" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> GetOrAddNew(StreamProcessorId streamProcessorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the <see cref="StreamPosition" /> to point to the next event to process for a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorId">The unique<see cref="StreamProcessorId" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="nextEventToProcessPosition">The <see cref="StreamPosition" /> of the next event to process.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> SetNextEventToProcessPosition(StreamProcessorId streamProcessorId, StreamPosition nextEventToProcessPosition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a failing partition to the state.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="position">The <see cref="StreamPosition" /> of the failing event.</param>
        /// <param name="failureType">The <see cref="ProcessorFailureType" />.</param>
        /// <param name="retryTime">The <see cref="DateTimeOffset" /> point in time to retry processing.</param>
        /// <param name="reason">The reason for failure.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> AddFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, StreamPosition position, ProcessorFailureType failureType, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a failing partition to the state.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> RemoveFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the <see cref="FailingPartitionState" /> of for a partition.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="failingPartitionState">The <see cref="FailingPartitionState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> SetFailingPartitionState(StreamProcessorId streamProcessorId, PartitionId partitionId, FailingPartitionState failingPartitionState, CancellationToken cancellationToken = default);
    }
}