// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.when_projecting
{
    public class and_the_request_factory_fails : given.all_dependencies
    {
        static Exception error;

        Establish context = () =>
        {
            error = new Exception();
            request_factory
                .Setup(_ => _.TryCreate(current_state, @event))
                    .Returns(Try<EmbeddingRequest>.Failed(error));
        };

        static Exception result;

        Because of = () => result = Catch.Exception(() => embedding.Project(current_state, @event, cancellation).GetAwaiter().GetResult());

        It should_have_called_the_request_factory = ()
            => request_factory.Verify(_ => _.TryCreate(current_state, @event));
        It should_not_do_anything_with_the_dispatcher = () => dispatcher.VerifyNoOtherCalls();
        It should_return_the_correct_error = () => result.ShouldEqual(error);
    }
}
