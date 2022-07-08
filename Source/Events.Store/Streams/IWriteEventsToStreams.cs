// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines a system that can write an event to a stream in the event store.
/// </summary>
public interface IWriteEventsToStreams
{
    /// <summary>
    /// Writes an event to a stream.
    /// </summary>
    /// <param name="event">The <see cref="CommittedEvent" />.</param>
    /// <param name="scope">The <see cref="ScopeId" />.</param>
    /// <param name="streamId">The <see cref="StreamId" />.</param>
    /// <param name="partitionId">The <see cref="PartitionId" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> representing whether the event was successfully written to the event store.</returns>
    Task Write(CommittedEvent @event, ScopeId scope, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Writes events to a stream.
    /// </summary>
    /// <param name="events">The <see cref="CommittedEvent" /> events to commit.</param>
    /// <param name="scope">The <see cref="ScopeId" />.</param>
    /// <param name="streamId">The <see cref="StreamId" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> representing whether the event was successfully written to the event store.</returns>
    Task Write(IEnumerable<(CommittedEvent, PartitionId)> events, ScopeId scope, StreamId streamId, CancellationToken cancellationToken);
}
