// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.given
{
    public class all_dependencies
    {
        protected static EmbeddingId embedding;
        protected static Mock<IUpdateEmbeddingStates> state_updater;
        protected static Mock<IWaitForAggregateRootEvents> event_waiter;
        protected static Mock<IEventStore> event_store;
        protected static Mock<IEmbeddingStore> embedding_store;
        protected static Mock<ICalculateStateTransitionEvents> transition_calculator;
        protected static EmbeddingProcessor embedding_processor;
        protected static CancellationToken cancellation_token;

        Establish context = () =>
        {
            embedding = "f46237c8-e144-4f29-bdcc-610ba075735b";
            state_updater = new Mock<IUpdateEmbeddingStates>();
            event_waiter = new Mock<IWaitForAggregateRootEvents>();
            event_store = new Mock<IEventStore>();
            embedding_store = new Mock<IEmbeddingStore>();
            transition_calculator = new Mock<ICalculateStateTransitionEvents>();
            embedding_processor = new EmbeddingProcessor(
                embedding,
                state_updater.Object,
                event_waiter.Object,
                event_store.Object,
                embedding_store.Object,
                transition_calculator.Object);
            cancellation_token = CancellationToken.None;

            state_updater.Setup(_ => _.TryUpdateAll(It.IsAny<CancellationToken>())).Returns(Task.FromResult(Try.Succeeded()));
            event_waiter.Setup(_ => _.WaitForEvent(embedding.Value, It.IsAny<CancellationToken>())).Returns<ArtifactId, CancellationToken>((_, cancellationToken) => Task.Delay(Timeout.Infinite, cancellationToken));
        };
    }
}
