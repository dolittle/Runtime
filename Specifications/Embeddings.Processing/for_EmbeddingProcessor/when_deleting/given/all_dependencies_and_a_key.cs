// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using CommittedAggregateEvents = Dolittle.Runtime.Events.Store.CommittedAggregateEvents;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;
using UncommittedEvent = Dolittle.Runtime.Events.Store.UncommittedEvent;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_deleting.given;

public class all_dependencies_and_a_key : for_EmbeddingProcessor.given.all_dependencies
{
    protected static UncommittedEvent uncommitted_event;
    protected static ProjectionKey key;
    protected static EmbeddingCurrentState current_state;
    protected static AggregateRootVersion aggregate_root_version;
    protected static CommittedAggregateEvent committed_event;
    protected static CommittedAggregateEvents committed_events;
    Establish context = () =>
    {
        key = "projection-key";
        current_state = new EmbeddingCurrentState(1, EmbeddingCurrentStateType.Persisted, "{}", key);
        uncommitted_event = new UncommittedEvent("1f5e6a1b-f187-4060-8dc2-4a93fff4cd1d", new Artifact("24eca2e4-dd4b-45d0-8808-e6833d3680ca", 2), false, "content");
        aggregate_root_version = 1;
        committed_event = new CommittedAggregateEvent(
            new Artifact("5512cda5-5e38-4654-ba86-3a7d917f3eb0", ArtifactGeneration.First),
            0,
            0,
            DateTimeOffset.Now,
            "1d137f3a-b8d0-43a5-a08a-f8eb35b5e932",
            new ExecutionContext(
                Guid.Parse("4e93a0f0-ddf1-48a3-ab6e-508ed9950ed4"),
                "63560704-69dd-47bd-a4de-41af2634a190",
                new Version(1, 2, 3),
                "production",
                Guid.Parse("99004ad1-0cbb-48b0-8e43-796e9c7c45a0"),
                ActivitySpanId.CreateRandom(),
                Claims.Empty,
                CultureInfo.InvariantCulture),
            new Artifact("37e282a0-5597-4829-9508-3204f3adb92e", ArtifactGeneration.First),
            false,
            "event-content");
        committed_events = new CommittedAggregateEvents(
            "1d137f3a-b8d0-43a5-a08a-f8eb35b5e932",
            "5512cda5-5e38-4654-ba86-3a7d917f3eb0",
            aggregate_root_version,
            new[] { committed_event });
    };
    protected static UncommittedAggregateEvents CreateUncommittedEvents(params UncommittedEvent[] events)
        => new(
            "1d137f3a-b8d0-43a5-a08a-f8eb35b5e932",
            new Artifact("5512cda5-5e38-4654-ba86-3a7d917f3eb0", ArtifactGeneration.First),
            1,
            events);

}