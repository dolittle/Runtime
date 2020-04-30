// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a system that can fetch <see cref="StreamEvent">events</see> from <see cref="StreamId">streams</see>.
    /// </summary>
    public interface ICanFetchEventsFromPartitionedStream : ICanFetchEventsFromStream
    {
        /// <summary>
        /// Fetch the first event in the given partition from a given <see cref="StreamPosition" />.
        /// </summary>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="streamPosition"><see cref="StreamPosition">the position in the stream</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="StreamEvent" />.</returns>
        Task<StreamEvent> FetchInPartition(PartitionId partitionId, StreamPosition streamPosition, CancellationToken cancellationToken);
    }
}
