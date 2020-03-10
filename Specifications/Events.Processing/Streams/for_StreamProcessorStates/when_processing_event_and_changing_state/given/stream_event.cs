// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorStates.when_processing_event_and_changing_state.given
{
    public static class stream_event
    {
        public static StreamEvent single(PartitionId partition) =>
            new StreamEvent(
                new CommittedEvent(
                    0,
                    DateTimeOffset.Now,
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    new Cause(CauseType.Command, 0),
                    new Artifacts.Artifact(Guid.NewGuid(), 1),
                    false,
                    ""),
                Guid.NewGuid(),
                partition);
    }
}