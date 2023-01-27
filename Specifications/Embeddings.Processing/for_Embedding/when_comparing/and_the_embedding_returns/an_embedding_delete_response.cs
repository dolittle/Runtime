// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_comparing.and_the_embedding_returns;

public class an_embedding_delete_response : given.all_dependencies
{
    static EmbeddingResponse embedding_response;
    static Events.Contracts.UncommittedEvent pb_uncommitted_event;

    Establish context = () =>
    {
        pb_uncommitted_event = new Events.Contracts.UncommittedEvent
        {
            Content = "{}",
            EventSourceId = "302c7c8f-b773-44e7-947d-75747ed1a976",
            Public = true,
            EventType = new Dolittle.Artifacts.Contracts.Artifact()
            {
                Id = Guid.Parse("0a4b2ee3-a85f-4d53-a3b9-c87cc326a58b").ToProtobuf(),
                Generation = 0
            }
        };
        embedding_response = new EmbeddingResponse
        {
            Delete = new EmbeddingDeleteResponse()
        };

        embedding_response.Delete.Events.AddRange(new[] { pb_uncommitted_event });

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

    Because of = () => result = embedding.TryCompare(current_state, desired_state, execution_context,cancellation).GetAwaiter().GetResult();

    It should_call_the_dispatcher = () => dispatcher.Verify(_ => _.Call(embedding_request, execution_context, cancellation), Times.Once);
    It should_not_do_anything_more_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
    It should_return_a_failure_result = () => result.Success.Should().BeFalse();
    It should_fail_because_unexpected_response_case = () => result.Exception.Should().BeOfType<UnexpectedEmbeddingResponse>();
    It should_have_called_the_request_factory = () => request_factory.Verify(_ => _.TryCreate(current_state, desired_state));
}