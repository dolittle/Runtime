// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines how we get and set the <see cref="StreamProcessorState"/>for <see cref="StreamProcessor" >stream processors</see>.
    /// </summary>
    public interface IStreamProcessorStateRepository : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="StreamProcessorState" /> for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorId">The unique<see cref="StreamProcessorId" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> Get(StreamProcessorId streamProcessorId);

        /// <summary>
        /// Increments the <see cref="StreamPosition" /> for a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorKey">The unique<see cref="StreamProcessorId" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> IncrementPosition(StreamProcessorId streamProcessorKey);

        /// <summary>
        /// Adds a failing partition to the state.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="currentState">The current <see cref="StreamProcessorState" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="retryTime">The <see cref="DateTimeOffset" /> point in time to retry processing.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> AddFailingPartition(StreamProcessorId streamProcessorId, StreamProcessorState currentState, PartitionId partitionId, DateTimeOffset retryTime);

        /// <summary>
        /// Adds a failing partition to the state.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> RemoveFailingPartition(StreamProcessorId streamProcessorId, PartitionId partitionId);

        /// <summary>
        /// Sets the <see cref="FailingPartitionState" /> of for a partition.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="failingPartitionState">The <see cref="FailingPartitionState" />.</param>
        /// <returns>The persisted <see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> SetFailingPartitionState(StreamProcessorId streamProcessorId, PartitionId partitionId, FailingPartitionState failingPartitionState);
    }
}