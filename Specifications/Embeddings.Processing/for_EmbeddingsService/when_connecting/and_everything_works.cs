// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;
using Machine.Specifications;
using static Moq.It;
using static Moq.Times;
namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.when_connecting;

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
                IsAny<CancellationToken>()
            ))
            .Returns(Task.FromResult<Try<(IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>, EmbeddingRegistrationArguments)>>((dispatcher.Object, new EmbeddingRegistrationArguments(execution_context, embedding_definition))));
        embedding_processors
            .Setup(_ => _.TryStartEmbeddingProcessorForAllTenants(embedding_id, IsAny<CreateEmbeddingProcessorForTenant>(), IsAny<CancellationToken>()))
            .Returns(processor_tcs.Task);
        embedding_processors
            .Setup(_ => _.HasEmbeddingProcessors(embedding_id))
            .Returns(false);
        dispatcher
            .Setup(_ => _.Accept(IsAny<EmbeddingRegistrationResponse>(), IsAny<CancellationToken>()))
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
            IsAny<CancellationToken>()),
        Once);

    It should_set_the_execution_context = () => execution_context_manager.Verify(_ => _.CurrentFor(execution_context, IsAny<string>(), IsAny<int>(), IsAny<string>()), Once);
    It should_have_registered_the_embedding = () => embedding_processors.Verify(
        _ => _.TryStartEmbeddingProcessorForAllTenants(
            embedding_id,
            IsAny<CreateEmbeddingProcessorForTenant>(),
            IsAny<CancellationToken>()),
        Once);

    It should_have_validated_definition = () => embedding_definition_comparer.Verify(_ => _.DiffersFromPersisted(embedding_definition, IsAny<CancellationToken>()), Once);
    It should_have_persisted_definition = () => embedding_definition_persister.Verify(_ => _.TryPersist(embedding_definition, IsAny<CancellationToken>()), Once);

    It should_have_accepted_reverse_call = () => dispatcher.Verify(
        _ => _.Accept(
            Is<EmbeddingRegistrationResponse>(response => response.Failure == default),
            IsAny<CancellationToken>()),
        Once);

    It should_not_reject_reverse_call = () => dispatcher.Verify(
        _ => _.Reject(IsAny<EmbeddingRegistrationResponse>(), IsAny<CancellationToken>()), Never);

    Cleanup clean = () => processor_tcs.SetResult(Try.Succeeded());

}