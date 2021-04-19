// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_updating.given
{
    public class all_dependencies_and_a_desired_state : for_EmbeddingProcessor.given.all_dependencies
    {
        protected static ProjectionKey key;
        protected static EmbeddingCurrentState current_state;
        protected static UncommittedAggregateEvents uncommitted_events;
        protected static CommittedAggregateEvents committed_events;
        protected static ProjectionState desired_state;
        Establish context = () =>
        {
            key = "projection-key";
            current_state = new EmbeddingCurrentState(1, ProjectionCurrentStateType.Persisted, "{}", key);
            uncommitted_events = new UncommittedAggregateEvents(
                Guid.Parse("1d137f3a-b8d0-43a5-a08a-f8eb35b5e932"),
                new Artifact("5512cda5-5e38-4654-ba86-3a7d917f3eb0", ArtifactGeneration.First),
                1,
                new List<UncommittedEvent>());
            committed_events = new CommittedAggregateEvents(
                Guid.Parse("1d137f3a-b8d0-43a5-a08a-f8eb35b5e932"),
                "5512cda5-5e38-4654-ba86-3a7d917f3eb0",
                new List<CommittedAggregateEvent>());
            desired_state = "{}";
        };
    }
}