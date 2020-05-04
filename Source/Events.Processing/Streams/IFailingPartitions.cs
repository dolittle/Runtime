// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a system that knows about <see cref="FailingPartitionState">failing partition states </see>.
    /// </summary>
    public interface IFailingPartitions
    {
        /// <summary>
        /// Adds a <see cref="FailingPartitionState" /> for a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="partition">The <see cref="PartitionId" /> that failed.</param>
        /// <param name="position">The <see cref="StreamPosition" /> it failed on.</param>
        /// <param name="retryTime">The <see cref="DateTimeOffset" /> it will retry processing again.</param>
        /// <param name="reason">The reason it failed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The new <see cref="StreamProcessorState" />.</returns>
        Task<StreamProcessorState> AddFailingPartitionFor(StreamProcessorId streamProcessorId, PartitionId partition, StreamPosition position, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken);

        /// <summary>
        /// Catchup all failing partitions for a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId"/>.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="streamProcessorState">The current <see cref="StreamProcessorState" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The new <see cref="StreamProcessorState" />.</returns>
        Task<StreamProcessorState> CatchupFor(StreamProcessorId streamProcessorId, IEventProcessor eventProcessor, StreamProcessorState streamProcessorState, CancellationToken cancellationToken);
    }
}