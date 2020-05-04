// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a system that can fetch <see cref="StreamEvent">events</see> from <see cref="StreamId">streams</see>.
    /// </summary>
    public interface IFetchEventsFromStreams
    {
        /// <summary>
        /// Fetch the event at a given <see cref="StreamPosition" /> in a stream.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="streamId"><see cref="StreamId">the stream in the event store</see>.</param>
        /// <param name="streamPosition"><see cref="StreamPosition">the position in the stream</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="StreamEvent" />.</returns>
        Task<StreamEvent> Fetch(ScopeId scope, StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the <see cref="StreamPosition" /> of the first event in a <see cref="StreamId" >stream</see> in a specific <see cref="PartitionId" /> starting search from the given <see cref="StreamPosition" />..
        /// </summary>
        /// <remarks>If there are no more events in the stream from the given <see cref="StreamPosition" /> with the given <see cref="PartitionId" />, this will return the maximum value of <see cref="StreamPosition" />.</remarks>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="fromPosition">The <see cref="StreamPosition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="StreamPosition" /> of the first event with the given <see cref="PartitionId" /> from the given <see cref="StreamPosition" /> or returns the maximum value of <see cref="StreamPositionRange" /> if .</returns>
        Task<StreamPosition> FindNext(ScopeId scope, StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken);
    }
}
