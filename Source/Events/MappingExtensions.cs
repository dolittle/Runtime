// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events;

public static class MappingExtensions
{
    public static CommittedEvent FromProtobuf(this Contracts.CommittedEvent committedEvent)
    {
        return new CommittedEvent(
            committedEvent.EventLogSequenceNumber,
            committedEvent.Occurred.ToDateTimeOffset(),
            committedEvent.EventSourceId,
            committedEvent.ExecutionContext.ToExecutionContext(),
            committedEvent.EventType.ToArtifact(),
            committedEvent.Public,
            committedEvent.Content);
            
    }
}
