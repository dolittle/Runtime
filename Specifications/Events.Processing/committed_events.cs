// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing;

public static class committed_events
{
    public static CommittedEvent single() => single(EventLogSequenceNumber.Initial);
    
    public static CommittedEvent single(EventLogSequenceNumber event_log_sequence_number) => single(event_log_sequence_number, "{\"something\":42}");
    public static CommittedEvent single(string content) => single(EventLogSequenceNumber.Initial, content);
    public static CommittedEvent single(EventLogSequenceNumber event_log_sequence_number, string content) => new(
        event_log_sequence_number,
        DateTimeOffset.UtcNow,
        "event source///",
        execution_contexts.create(),
        new Artifact(ArtifactId.New(), ArtifactGeneration.First),
        false,
        content);
}