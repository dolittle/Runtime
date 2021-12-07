// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_projecting;

public class and_the_request_factory_fails : given.all_dependencies
{
    static Exception error;

    Establish context = () =>
    {
        error = new Exception();
        request_factory
            .Setup(_ => _.Create(current_state, @event))
            .Throws(error);
    };


    static IProjectionResult result;

    Because of = () => result = embedding.Project(current_state, @event, cancellation).GetAwaiter().GetResult();

    It should_have_called_the_request_factory = ()
        => request_factory.Verify(_ => _.Create(current_state, @event));
    It should_not_do_anything_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
    It should_return_failed_result = () => result.ShouldBeOfExactType<ProjectionFailedResult>();
    It should_return_fail_because_remote_call_failed = () => ((ProjectionFailedResult)result).Exception.ShouldEqual(error);
}