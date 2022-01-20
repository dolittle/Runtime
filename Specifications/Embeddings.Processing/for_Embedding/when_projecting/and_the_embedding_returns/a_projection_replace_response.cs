// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Processing.Projections;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_projecting.and_the_embedding_returns;

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
            .Setup(_ => _.Create(current_state, @event))
            .Returns(embedding_request);
        dispatcher
            .Setup(_ => _.Call(
                embedding_request,
                cancellation))
            .Returns(Task.FromResult(embedding_response));
    };

    static IProjectionResult result;

    Because of = () => result = embedding.Project(current_state, @event, cancellation).GetAwaiter().GetResult();

    It should_call_the_dispatcher = ()
        => dispatcher.Verify(_ => _.Call(embedding_request, cancellation), Moq.Times.Once);
    It should_not_do_anything_more_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
    It should_return_a_projection_replace_result = ()
        => result.ShouldBeOfExactType<ProjectionReplaceResult>();
    It should_have_the_received_state_in_the_result = ()
        => (result as ProjectionReplaceResult).State.Value.ShouldEqual(received_state);
    It should_have_called_the_request_factory = ()
        => request_factory.Verify(_ => _.Create(current_state, @event));
}