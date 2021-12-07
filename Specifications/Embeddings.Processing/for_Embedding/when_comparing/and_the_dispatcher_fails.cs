// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_comparing;

public class and_the_dispatcher_fails : given.all_dependencies
{
    static Exception error;

    Establish context = () =>
    {
        error = new Exception();
        request_factory
            .Setup(_ => _.TryCreate(current_state, desired_state))
            .Returns(embedding_request);
        dispatcher
            .Setup(_ => _.Call(
                embedding_request,
                cancellation))
            .Returns(Task.FromException<EmbeddingResponse>(error));
    };

    static Try<UncommittedEvents> result;

    Because of = () => result = embedding.TryCompare(current_state, desired_state, cancellation).GetAwaiter().GetResult();

    It should_call_the_dispatcher = () => dispatcher.Verify(_ => _.Call(embedding_request, cancellation), Times.Once);
    It should_not_do_anything_more_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
    It should_return_failure = () => result.Success.ShouldBeFalse();
    It should_return_the_correct_error = () => result.Exception.ShouldEqual(error);
}