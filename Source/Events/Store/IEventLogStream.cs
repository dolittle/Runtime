// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

public record EventLogBatch(EventLogSequenceNumber From, EventLogSequenceNumber To, IReadOnlyList<Contracts.CommittedEvent> MatchedEvents);

/// <summary>
/// Defines the event log as something that can be subscribed to. 
/// </summary>
public interface IEventLogStream
{
    /// <summary>
    /// Subscribe to the event log stream from the given offset, filtered by event types.
    /// </summary>
    /// <param name="scope">The <see cref="ScopeId"/>.</param>
    /// <param name="from">From offset, inclusive</param>
    /// <param name="eventTypes">Included event types, min 1</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ChannelReader<EventLogBatch> Subscribe(
        ScopeId scope,
        EventLogSequenceNumber from,
        IReadOnlyCollection<ArtifactId> eventTypes,
        CancellationToken cancellationToken);

    /// <summary>
    /// Subscribe to the complete event log stream at the given offset
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="from">From offset, inclusive</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ChannelReader<EventLogBatch> SubscribeAll(ScopeId scopeId, EventLogSequenceNumber from, CancellationToken cancellationToken);
    
    /// <summary>
    /// Subscribe to the complete event log stream at the given offset, including public events only
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="from">From offset, inclusive</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ChannelReader<EventLogBatch> SubscribePublic(EventLogSequenceNumber from, CancellationToken cancellationToken);
}
