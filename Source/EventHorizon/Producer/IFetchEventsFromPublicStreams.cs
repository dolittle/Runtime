// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Defines a system that can fetch events from public event streams.
    /// </summary>
    public interface IFetchEventsFromPublicStreams : IFetchEventsFromStreams
    {
        /// <summary>
        /// Fetch the event at a given position in a stream.
        /// </summary>
        /// <param name="streamId"><see cref="StreamId">the stream in the event store</see>.</param>
        /// <param name="streamPosition"><see cref="StreamPosition">the position in the stream</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="StreamEvent" />.</returns>
        Task<StreamEvent> Fetch(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetch a range of events from a position to another in a stream.
        /// </summary>
        /// <param name="streamId"><see cref="StreamId">the stream in the event store</see>.</param>
        /// <param name="range">The <see cref="StreamPositionRange" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IEnumerable{T}" /> of <see cref="StreamEvent" />.</returns>
        Task<IEnumerable<StreamEvent>> FetchRange(StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken = default);

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
