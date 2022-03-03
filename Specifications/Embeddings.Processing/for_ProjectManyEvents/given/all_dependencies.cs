// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Embeddings.Processing.for_ProjectManyEvents.given;

public class all_dependencies
{
    protected static EmbeddingId identifier;
    protected static ExecutionContext execution_context;
    protected static Artifact event_type;
    protected static Mock<IEmbedding> embedding;
    protected static ProjectionState initial_state;
    protected static ProjectManyEvents project_many_events;
    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        identifier = "f95434e0-46e4-48bc-9881-f7379b3e6c8b";
        execution_context = new ExecutionContext(
            Guid.Parse("d62477c1-13f2-4c60-95ad-acadf145b48e"),
            "3d680170-1633-4e8e-8de7-1b5f673ac70e",
            new Version(9, 8, 7),
            "production",
            Guid.Parse("28f65310-4902-4331-a373-9be8f9668b75"),
            Claims.Empty,
            CultureInfo.InvariantCulture);
        event_type = new Artifact(Guid.Parse("f323d03b-4ba9-4a06-823f-c2f82fa5fc89"), ArtifactGeneration.First);
        embedding = new Mock<IEmbedding>(MockBehavior.Strict);
        initial_state = "projection-initial-state";
        project_many_events = new ProjectManyEvents(
            identifier,
            embedding.Object,
            initial_state,
            Mock.Of<ILogger>());
        cancellation_token = CancellationToken.None;
    };
}