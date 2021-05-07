// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_projecting
{
    public class and_the_embedding_returns_a_failure : given.all_dependencies
    {
        static EmbeddingResponse embedding_response;
        static string failure_reason;
        Establish context = () =>
        {
            failure_reason = "sometimes 🙅🕒 you 👆😤 do 💕 be 💪🐝 a 😯 failure";
            embedding_response = new EmbeddingResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                {
                    Id = new FailureId(Guid.Parse("1f10056e-9cb8-4f8d-8764-01a8cb6b75f8")).ToProtobuf(),
                    Reason = failure_reason
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
            // while (!System.Diagnostics.Debugger.IsAttached) System.Threading.Thread.Sleep(10);


        };

        static IProjectionResult result;

        Because of = () => result = embedding.Project(current_state, @event, cancellation).GetAwaiter().GetResult();

        It should_have_called_the_request_factory = ()
            => request_factory.Verify(_ => _.Create(current_state, @event));

        It should_call_the_dispatcher = ()
            => dispatcher.Verify(_ => _.Call(embedding_request, cancellation), Times.Once);

        It should_not_do_anything_more_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
        It should_return_a_projection_failed_result = ()
            => result.ShouldBeOfExactType<ProjectionFailedResult>();

        It should_return_a_projection_failed_result_with_correct_exception_type = ()
            => (result as ProjectionFailedResult).Exception.ShouldBeOfExactType<ProjectionFailed>();

        It should_have_the_failure_reason_in_the_result = ()
            => (result as ProjectionFailedResult).Exception.Message
                .ShouldEqual(new ProjectionFailed(failure_reason).Message);
    }
}
