// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;

/// <summary>
/// Defines a system that can write events to public event streams.
/// </summary>
public interface IWriteEventsToPublicStreams : IWriteEventsToStreams
{
    /// <summary>
    /// Writes an event to a public stream.
    /// </summary>
    /// <param name="event">The <see cref="CommittedEvent" />.</param>
    /// <param name="streamId">The <see cref="StreamId" />.</param>
    /// <param name="partitionId">The <see cref="PartitionId" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> representing whether the event was successfully written to the event store.</returns>
    Task Write(CommittedEvent @event, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken);
}