// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.when_connecting
{
    public class and_everything_works : given.all_dependencies
    {
        static TaskCompletionSource<Try> processor_tcs;
        static TaskCompletionSource dispatcher_tcs;
        Establish context = () =>
        {
            processor_tcs = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
            dispatcher_tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
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
                .Setup(_ => _.TryStartEmbeddingProcessorForAllTenants(embedding_id, Moq.It.IsAny<CreateEmbeddingProcessorForTenant>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(processor_tcs.Task);
            embedding_processors
                .Setup(_ => _.HasEmbeddingProcessors(embedding_id))
                .Returns(false);
            dispatcher
                .Setup(_ => _.Accept(Moq.It.IsAny<EmbeddingRegistrationResponse>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(dispatcher_tcs.Task);
        };

        static Task result;

        Because of = () =>
        {
            result = embedding_service.Connect(runtime_stream.Object, client_stream.Object, call_context);
            Task.Delay(50).GetAwaiter().GetResult();
        };

        It should_not_be_completed = () => result.IsCompleted.ShouldBeFalse();
        It should_have_connected_the_reverse_call_client = () => reverse_call_services.Verify(
            _ => _.Connect(
                runtime_stream.Object,
                client_stream.Object,
                call_context,
                protocol,
                Moq.It.IsAny<CancellationToken>()),
            Moq.Times.Once);

        It should_set_the_execution_context = () => execution_context_manager.Verify(_ => _.CurrentFor(execution_context, Moq.It.IsAny<string>(), Moq.It.IsAny<int>(), Moq.It.IsAny<string>()), Moq.Times.Once);
        It should_have_registered_the_embedding = () => embedding_processors.Verify(
            _ => _.TryStartEmbeddingProcessorForAllTenants(
                embedding_id,
                Moq.It.IsAny<CreateEmbeddingProcessorForTenant>(),
                Moq.It.IsAny<CancellationToken>()),
            Moq.Times.Once);

        It should_have_accepted_reverse_call = () => dispatcher.Verify(
            _ => _.Accept(
                Moq.It.Is<EmbeddingRegistrationResponse>(response => response.Failure == default),
                Moq.It.IsAny<CancellationToken>()),
            Moq.Times.Once);

        Cleanup clean = () => processor_tcs.SetResult(Try.Succeeded());

    }
}