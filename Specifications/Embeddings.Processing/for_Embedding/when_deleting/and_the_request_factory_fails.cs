// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_deleting;

public class and_the_request_factory_fails : given.all_dependencies
{
    static Exception error;

    Establish context = () =>
    {
        error = new Exception();
        request_factory
            .Setup(_ => _.TryCreate(current_state))
            .Returns(error);
    };

    static Try<UncommittedEvents> result;

    Because of = () => result = embedding.TryDelete(current_state, execution_context, cancellation).GetAwaiter().GetResult();

    It should_have_called_the_request_factory = ()
        => request_factory.Verify(_ => _.TryCreate(current_state));
    It should_not_do_anything_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
    It should_return_failure = () => result.Success.Should().BeFalse();
    It should_return_the_correct_error = () => result.Exception.Should().Be(error);
}