// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
    /// <summary>
    /// Represents a system that knows about <see cref="FailingPartitionState">failing partition states </see>.
    /// </summary>
    public interface IFailingPartitions
    {
        /// <summary>
        /// Adds a <see cref="FailingPartitionState" /> for a <see cref="ScopedStreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        /// <param name="oldState">The old <see cref="StreamProcessorState" />.</param>
        /// <param name="failedPosition">The position where the processing failed.</param>
        /// <param name="partition">The <see cref="PartitionId" /> that failed.</param>
        /// <param name="retryTime">The <see cref="DateTimeOffset" /> it will retry processing again.</param>
        /// <param name="reason">The reason it failed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="StreamProcessorState" />.</returns>
        Task<IStreamProcessorState> AddFailingPartitionFor(IStreamProcessorId streamProcessorId, StreamProcessorState oldState, StreamPosition failedPosition, PartitionId partition, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken);

        /// <summary>
        /// Catchup all failing partitions for a <see cref="ScopedStreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId"/>.</param>
        /// <param name="streamProcessorState">The current <see cref="StreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the new <see cref="StreamProcessorState" />.</returns>
        Task<IStreamProcessorState> CatchupFor(IStreamProcessorId streamProcessorId, StreamProcessorState streamProcessorState, CancellationToken cancellationToken);
    }
}