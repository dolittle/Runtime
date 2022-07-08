// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_comparing.and_the_embedding_returns;

public class a_projection_replace_response : given.all_dependencies
{
    static EmbeddingResponse embedding_response;
    static string received_state;
    Establish context = () =>
    {
        received_state = "im the 👏👑 received state 🗽🏛";

        embedding_response = new EmbeddingResponse
        {
            ProjectionReplace = new Events.Processing.Contracts.ProjectionReplaceResponse
            {
                State = received_state
            }
        };

        request_factory
            .Setup(_ => _.TryCreate(current_state, desired_state))
            .Returns(embedding_request);
        dispatcher
            .Setup(_ => _.Call(
                embedding_request,
                execution_context,
                cancellation))
            .Returns(Task.FromResult(embedding_response));
    };

    static Try<UncommittedEvents> result;

    Because of = () => result = embedding.TryCompare(current_state, desired_state, execution_context, cancellation).GetAwaiter().GetResult();

    It should_call_the_dispatcher = () => dispatcher.Verify(_ => _.Call(embedding_request, execution_context, cancellation), Times.Once);
    It should_not_do_anything_more_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
    It should_return_a_failure_result = () => result.Success.ShouldBeFalse();
    It should_fail_because_unexpected_response_case = () => result.Exception.ShouldBeOfExactType<UnexpectedEmbeddingResponse>();
    It should_have_called_the_request_factory = () => request_factory.Verify(_ => _.TryCreate(current_state, desired_state));
}