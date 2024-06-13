// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Channels;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// The message that is sent from <see cref="IStreamEventSubscriber"/> when a new event is available.
/// If the event is not of a subscribed type, it will send the next upcoming <see cref="EventLogSequenceNumber"/> instead.
/// </summary>
public readonly struct StreamSubscriptionMessage
{
    public StreamEvent? StreamEvent { get; }

    public EventLogSequenceNumber? NextEventLogSequenceNumber { get; }

    public StreamSubscriptionMessage(StreamEvent @event)
    {
        StreamEvent = @event;
        IsEvent = true;
    }

    public StreamSubscriptionMessage(EventLogSequenceNumber nextEventLogSequenceNumber)
    {
        NextEventLogSequenceNumber = nextEventLogSequenceNumber;
        IsEvent = false;
    }

    [MemberNotNullWhen(true, nameof(StreamEvent))]
    [MemberNotNullWhen(false, nameof(NextEventLogSequenceNumber))]
    public bool IsEvent { get; }
}


public interface IStreamEventSubscriber
{
    /// <summary>
    /// Subscribe to a stream of events for a specific scope and a set of event types.
    /// If there are new events where none of the types are subscribed to,
    /// it will send the next upcoming EventLogSequenceNumber instead.
    /// </summary>
    /// <param name="scopeId">The source scope</param>
    /// <param name="artifactIds">The set of subscribed events</param>
    /// <param name="from">The <see cref="ProcessingPosition"/> to continue from (inclusive)</param>
    /// <param name="partitioned">Whether the events should be tagged with "partitioned"</param>
    /// <param name="subscriptionName">Identifier for the given subscription, used for debugging only</param>
    /// <param name="until">Stops the subscription if the predicate returns true</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Either StreamEvent or the next EventLogSequenceNumber</returns>
    ChannelReader<StreamSubscriptionMessage> Subscribe(ScopeId scopeId,
        IReadOnlyCollection<ArtifactId> artifactIds,
        ProcessingPosition from,
        bool partitioned,
        string subscriptionName,
        Predicate<Contracts.CommittedEvent>? until,
        CancellationToken cancellationToken);
}
