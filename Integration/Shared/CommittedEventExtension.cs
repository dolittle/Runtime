// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using CommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Integration;

public static class CommittedEventExtension
{
    public static CommittedEvents ToCommittedEvents(this IEnumerable<CommittedEvent> committedEvents)
        => new(committedEvents.Select(_ => new Dolittle.Runtime.Events.Store.CommittedEvent(
            _.EventLogSequenceNumber,
            _.Occurred.ToDateTimeOffset(),
            _.EventSourceId,
            _.ExecutionContext.ToExecutionContext(),
            _.EventType.ToArtifact(),
            _.Public,
            _.Content)).ToList());
}
