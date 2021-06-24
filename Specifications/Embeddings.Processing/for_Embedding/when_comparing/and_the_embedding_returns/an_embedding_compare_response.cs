// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_comparing.and_the_embedding_returns
{
    public class an_embedding_compare_response : given.all_dependencies
    {
        static EmbeddingResponse embedding_response;
        static Events.Contracts.UncommittedEvent pb_uncommitted_event;

        Establish context = () =>
        {
            pb_uncommitted_event = new Events.Contracts.UncommittedEvent
            {
                Content = "{}",
                EventSourceId = Guid.Parse("302c7c8f-b773-44e7-947d-75747ed1a976").ToProtobuf(),
                Public = true,
                Artifact = new Dolittle.Artifacts.Contracts.Artifact()
                {
                    Id = Guid.Parse("0a4b2ee3-a85f-4d53-a3b9-c87cc326a58b").ToProtobuf(),
                    Generation = 0
                }
            };
            embedding_response = new EmbeddingResponse()
            {
                Compare = new EmbeddingCompareResponse()
            };

            embedding_response.Compare.Events.AddRange(new[] { pb_uncommitted_event });

            request_factory
                .Setup(_ => _.TryCreate(current_state, desired_state))
                .Returns(embedding_request);
            dispatcher
                .Setup(_ => _.Call(
                    embedding_request,
                    cancellation))
                .Returns(Task.FromResult(embedding_response));
        };

        static Try<UncommittedEvents> result;

        Because of = () => result = embedding.TryCompare(current_state, desired_state, cancellation).GetAwaiter().GetResult();

        It should_call_the_dispatcher = () => dispatcher.Verify(_ => _.Call(embedding_request, cancellation), Times.Once);
        It should_not_do_anything_more_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
        It should_return_a_successful_result = () => result.Success.ShouldBeTrue();
        It should_return_the_event = () =>
        {
            var events = result.Result;
            events.Count.ShouldEqual(1);
            events[0].Content.ShouldEqual(pb_uncommitted_event.Content);
            events[0].EventSource.ShouldEqual(EventSourceId.NotSet);
            events[0].Public.ShouldEqual(pb_uncommitted_event.Public);
            events[0].Type.Id.Value.ShouldEqual(pb_uncommitted_event.Artifact.Id.ToGuid());
            events[0].Type.Generation.Value.ShouldEqual(pb_uncommitted_event.Artifact.Generation);
        };
        It should_have_called_the_request_factory = () => request_factory.Verify(_ => _.TryCreate(current_state, desired_state));
    }
}
