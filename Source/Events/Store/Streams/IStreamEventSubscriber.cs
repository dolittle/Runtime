// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.Streams;

public interface IStreamEventSubscriber
{
    /// <summary>
    /// Subscribe to a stream of events for a specific scope and a set of event types.
    /// </summary>
    /// <param name="scopeId">The source scope</param>
    /// <param name="artifactIds">The set of subscribed events</param>
    /// <param name="from">The <see cref="ProcessingPosition"/> to continue from (inclusive)</param>
    /// <param name="partitioned">Whether the events should be tagged with "partitioned"</param>
    /// <param name="subscriptionName">Identifier for the given subscription, used for debugging only</param>
    /// <param name="until">Stops the subscription if the predicate returns true</param>
    /// <param name="cancellationToken"></param>
    /// <returns>(StreamEvent, (or null if no event of the correct type is in the current published batch), and the next processing position relevant for the consumer</returns>
    ChannelReader<(StreamEvent? streamEvent, EventLogSequenceNumber nextSequenceNumber)> Subscribe(ScopeId scopeId,
        IReadOnlyCollection<ArtifactId> artifactIds,
        ProcessingPosition from,
        bool partitioned,
        string subscriptionName,
        Predicate<Contracts.CommittedEvent>? until,
        CancellationToken cancellationToken);
}
