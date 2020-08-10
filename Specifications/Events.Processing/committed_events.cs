// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    public static class committed_events
    {
        public static CommittedEvent single() => new CommittedEvent(
                EventLogSequenceNumber.Initial,
                DateTimeOffset.UtcNow,
                EventSourceId.New(),
                execution_contexts.create(),
                new Artifact(ArtifactId.New(), ArtifactGeneration.First),
                false,
                "{\"something\":42}");

        public static CommittedEvent single(uint event_log_sequence_number) => new CommittedEvent(
                event_log_sequence_number,
                DateTimeOffset.UtcNow,
                EventSourceId.New(),
                execution_contexts.create(),
                new Artifact(ArtifactId.New(), ArtifactGeneration.First),
                false,
                "{\"something\":42}");
    }
}