// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Store;
using Grpc.Core;
using Grpc.Core.Testing;
using Machine.Specifications;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Embeddings.Store.Services.Grpc.for_EmbeddingStoreGrpcService.given;

public class the_service
{
    protected static EmbeddingId embedding;
    protected static ProjectionKey key;
    protected static ExecutionContext execution_context;
    protected static CancellationToken cancellation_token;
    protected static EmbeddingCurrentState a_current_state;
    protected static Mock<IEmbeddingsService> embeddings_service;
    protected static EmbeddingStoreGrpcService grpc_service;
    protected static ServerCallContext call_context;

    Establish context = () =>
    {
        embedding = "d6f99bc8-2430-4131-95d5-ea1540b260c3";
        key = "some key";
        execution_context = new ExecutionContext(
            "25bf2bca-bef9-4c25-99bb-8371c81eb9fa",
            "7134aeb9-2d15-486b-a9a4-e3b3509b3c18",
            Version.NotSet,
            "some env",
            "f6d7d582-81f1-4f37-acc8-f21324280c9a",
            Security.Claims.Empty,
            CultureInfo.InvariantCulture);
        cancellation_token = CancellationToken.None;
        a_current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "some state", "some key");
        embeddings_service = new Mock<IEmbeddingsService>();
        call_context = TestServerCallContext.Create(
            "method",
            null,
            DateTime.Now,
            Metadata.Empty,
            CancellationToken.None,
            "peer",
            null,
            null,
            _ => Task.CompletedTask,
            () => WriteOptions.Default,
            _ => { });
        grpc_service = new EmbeddingStoreGrpcService(embeddings_service.Object);
    };
}