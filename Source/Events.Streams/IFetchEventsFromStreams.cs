// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Defines a system that can fetch <see cref="Store.CommittedEvent">events</see> from <see cref="StreamId">streams</see>.
    /// </summary>
    public interface IFetchEventsFromStreams
    {
        /// <summary>
        /// Fetch the event at a given position in a stream.
        /// </summary>
        /// <param name="streamId"><see cref="StreamId">the stream in the event store</see>.</param>
        /// <param name="streamPosition"><see cref="StreamPosition">the position in the stream</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="CommittedEventWithPartition" />.</returns>
        Task<CommittedEventWithPartition> Fetch(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds the <see cref="StreamPosition" /> of the next event to process in a <see cref="StreamId" /> for a <see cref="PartitionId" />.
        /// </summary>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="fromPosition">The <see cref="StreamPosition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="StreamPosition" />of the next event to process.</returns>
        Task<StreamPosition> FindNext(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default);
    }
}