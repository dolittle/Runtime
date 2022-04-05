// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingStateUpdater.given;

public class all_dependencies
{
    protected static EmbeddingId embedding;
    protected static PartitionId events_partition;
    protected static ExecutionContext execution_context;
    protected static Artifact event_type;
    protected static Mock<IProjectManyEvents> project_many_events;
    protected static Mock<IFetchCommittedEvents> committed_events_fetcher;
    protected static Mock<IEmbeddingStore> embedding_store;
    protected static ProjectionState initial_state;
    protected static EmbeddingStateUpdater state_updater;
    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        embedding = "f95434e0-46e4-48bc-9881-f7379b3e6c8b";
        events_partition = "00000000-0000-0000-0000-000000000000";
        execution_context = new ExecutionContext(
            Guid.Parse("4e93a0f0-ddf1-48a3-ab6e-508ed9950ed4"),
            "63560704-69dd-47bd-a4de-41af2634a190",
            new Version(1, 2, 3),
            "production",
            Guid.Parse("99004ad1-0cbb-48b0-8e43-796e9c7c45a0"),
            Claims.Empty,
            CultureInfo.InvariantCulture);
        event_type = new Artifact(Guid.Parse("c57caeef-ce47-46f5-ad2e-6133833b1846"), ArtifactGeneration.First);
        project_many_events = new Mock<IProjectManyEvents>();
        committed_events_fetcher = new Mock<IFetchCommittedEvents>();
        embedding_store = new Mock<IEmbeddingStore>();
        initial_state = "projection-initial-state";
        state_updater = new EmbeddingStateUpdater(
            embedding,
            committed_events_fetcher.Object,
            embedding_store.Object,
            project_many_events.Object,
            Mock.Of<ILogger>());
        cancellation_token = CancellationToken.None;
    };
}