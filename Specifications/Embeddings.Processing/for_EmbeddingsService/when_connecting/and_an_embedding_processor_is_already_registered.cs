// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.when_connecting
{
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
                    Moq.It.IsAny<CancellationToken>()
                ))
                .Returns(Task.FromResult<Try<(IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>, EmbeddingRegistrationArguments)>>((dispatcher.Object, new EmbeddingRegistrationArguments(execution_context, embedding_id, events, initial_state))));
            embedding_processors
                .Setup(_ => _.HasEmbeddingProcessors(embedding_id))
                .Returns(true);
            dispatcher
                .Setup(_ => _.Reject(Moq.It.IsAny<EmbeddingRegistrationResponse>(), Moq.It.IsAny<CancellationToken>()))
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
                Moq.It.IsAny<CancellationToken>()),
            Moq.Times.Once);

        It should_set_the_execution_context = () => execution_context_manager.Verify(_ => _.CurrentFor(execution_context, Moq.It.IsAny<string>(), Moq.It.IsAny<int>(), Moq.It.IsAny<string>()), Moq.Times.Once);
        It should_reject_reverse_call = () => dispatcher.Verify(
            _ => _.Reject(
                Moq.It.IsAny<EmbeddingRegistrationResponse>(),
                Moq.It.IsAny<CancellationToken>()),
            Moq.Times.Once);
        It should_not_do_anything_else_with_dispatcher = () => dispatcher.VerifyNoOtherCalls();
        It should_check_whether_embedding_is_already_registered = () => embedding_processors.Verify(_ => _.HasEmbeddingProcessors(embedding_id), Moq.Times.Once);
        It should_not_do_anything_else_with_embedding_processors = () => embedding_processors.VerifyNoOtherCalls();

    }
}