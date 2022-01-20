// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;
using Machine.Specifications;
using static Moq.Times;
using static Moq.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.when_connecting;

public class and_an_embedding_processor_is_already_registered : given.all_dependencies
{
    Establish context = () =>
    {
        reverse_call_services
            .Setup(_ => _.Connect(
                runtime_stream.Object,
                client_stream.Object,
                call_context,
                protocol,
                IsAny<CancellationToken>()
            ))
            .Returns(Task.FromResult<Try<(IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>, EmbeddingRegistrationArguments)>>((dispatcher.Object, new EmbeddingRegistrationArguments(execution_context, embedding_definition))));
        embedding_processors
            .Setup(_ => _.HasEmbeddingProcessors(embedding_id))
            .Returns(true);
        dispatcher
            .Setup(_ => _.Reject(IsAny<EmbeddingRegistrationResponse>(), IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
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
            IsAny<CancellationToken>()),
        Once);

    It should_set_the_execution_context = () => execution_context_manager.Verify(_ => _.CurrentFor(execution_context, IsAny<string>(), IsAny<int>(), IsAny<string>()), Once);
    It should_reject_reverse_call = () => dispatcher.Verify(
        _ => _.Reject(
            IsAny<EmbeddingRegistrationResponse>(),
            IsAny<CancellationToken>()),
        Once);


    It should_not_persist_definition = () => embedding_definition_persister.Verify(_ => _.TryPersist(embedding_definition, IsAny<CancellationToken>()), Never);
    It should_not_do_anything_else_with_dispatcher = () => dispatcher.VerifyNoOtherCalls();
    It should_check_whether_embedding_is_already_registered = () => embedding_processors.Verify(_ => _.HasEmbeddingProcessors(embedding_id), Once);
    It should_not_do_anything_else_with_embedding_processors = () => embedding_processors.VerifyNoOtherCalls();

}