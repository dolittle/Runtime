// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;
using Machine.Specifications;
using static Moq.It;
using static Moq.Times;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.when_connecting;

public class and_reverse_call_connection_fails : given.all_dependencies
{
    Establish context = () =>
    {
        reverse_call_services
            .Setup(_ => _.Connect(
                runtime_stream.Object,
                client_stream.Object,
                call_context,
                protocol,
                IsAny<CancellationToken>(),
                false
            ))
            .Returns(Task.FromResult<Try<(IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>, EmbeddingRegistrationArguments)>>(new Exception()));
    };

    static Task result;

    Because of = () =>
    {
        result = embedding_service.Connect(runtime_stream.Object, client_stream.Object, call_context);
        Task.Delay(50).GetAwaiter().GetResult();
    };

    It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    It should_have_connected_the_reverse_call_client = () => reverse_call_services.Verify(
        _ => _.Connect(
            runtime_stream.Object,
            client_stream.Object,
            call_context,
            protocol,
            IsAny<CancellationToken>(),
            false),
        Once);

    It should_not_persist_definition = () => embedding_definition_persister.Verify(_ => _.TryPersist(embedding_definition, IsAny<CancellationToken>()), Never);
    It should_not_have_registered_the_embedding = () => embedding_processors.VerifyNoOtherCalls();
    It should_not_have_used_dispatcher = () => dispatcher.VerifyNoOtherCalls();

}