// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines a system that can fetch <see cref="StreamEvent">events</see> from <see cref="StreamId">streams</see>.
/// </summary>
public interface ICanFetchEventsFromPartitionedStream : ICanFetchEventsFromStream
{
    /// <summary>
    /// Fetch events in the given partition from a given <see cref="StreamPosition" />.
    /// </summary>
    /// <param name="partitionId">The <see cref="PartitionId" />.</param>
    /// <param name="position"><see cref="StreamPosition">the position in the stream</see>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The <see cref="Try{TResult}" /> with <see cref="StreamEvent" /> result.</returns>
    Task<Try<IEnumerable<StreamEvent>>> FetchInPartition(PartitionId partitionId, StreamPosition position, CancellationToken cancellationToken);

    /// <summary>
    /// Fetch events in the given partition from a given <see cref="StreamPosition" />.
    /// </summary>
    /// <param name="partitionId">The <see cref="PartitionId" />.</param>
    /// <param name="from"><see cref="StreamPosition">the from (inclusive) position in the stream</see>.</param>
    /// <param name="to"><see cref="StreamPosition">the to(exclusive position in the stream</see>.</param>
    /// <param name="artifactIds"></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The <see cref="Try{TResult}" /> with <see cref="StreamEvent" /> result.</returns>
    Task<(IList<StreamEvent> events, bool hasMoreEvents)> FetchInPartition(PartitionId partitionId, StreamPosition from, StreamPosition to, ISet<Guid> artifactIds,
        CancellationToken cancellationToken);

    Task<(StreamEvent? events, bool hasMoreEvents)> FetchNextEventInPartition(PartitionId partitionId, StreamPosition from, StreamPosition to,
        ISet<Guid> artifactIds,
        CancellationToken cancellationToken);
}
