// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_projecting.given;

public class all_dependencies : for_Embedding.given.all_dependencies
{
    protected static UncommittedEvent @event;
    protected static ProjectionCurrentState current_state;

    Establish context = () =>
    {
        @event = new UncommittedEvent(
            "1a477367-3404-45e6-a2af-6cbf19693b56",
            new Artifact(Guid.Parse("fe570d85-2619-49e4-bc72-9a8b2b2f149d"), ArtifactGeneration.First),
            true,
            "beautiful 😍 event to 🙅 be tested 🤓🤓");

        current_state = new ProjectionCurrentState(
            ProjectionCurrentStateType.Persisted,
            "projectionState",
            "projectionKey");

    };
}