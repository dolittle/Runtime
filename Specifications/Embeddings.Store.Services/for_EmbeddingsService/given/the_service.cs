// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Threading;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store;
using Machine.Specifications;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Embeddings.Store.Services.for_EmbeddingsService.given
{
    public class the_service
    {
        protected static EmbeddingId embedding;
        protected static ProjectionKey key;
        protected static ExecutionContext execution_context;
        protected static CancellationToken cancellation_token;
        protected static EmbeddingCurrentState a_current_state;
        protected static FactoryFor<IEmbeddingStore> get_embedding_store;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<IEmbeddingStore> embedding_store;
        protected static EmbeddingsService service;

        Establish context = () =>
        {
            execution_context_manager = new();
            embedding_store = new();
            get_embedding_store = () => embedding_store.Object;
            embedding = "d6f99bc8-2430-4131-95d5-ea1540b260c3";
            key = "some key";
            execution_context = new ExecutionContext(
                "25bf2bca-bef9-4c25-99bb-8371c81eb9fa",
                "7134aeb9-2d15-486b-a9a4-e3b3509b3c18",
                Versioning.Version.NotSet,
                "some env",
                "f6d7d582-81f1-4f37-acc8-f21324280c9a",
                Security.Claims.Empty,
                CultureInfo.InvariantCulture);
            cancellation_token = CancellationToken.None;
            a_current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "some state", "some key");
            service = new EmbeddingsService(get_embedding_store, execution_context_manager.Object);
        };
    }
}