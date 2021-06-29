// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_starting
{
    public class with_a_cancelled_token : given.all_dependencies
    {
        static CancellationToken cancellationToken;

        Establish context = () =>
        {
            cancellationToken = new CancellationToken(true);
        };

        static Task<Try> result;

        Because of = () => result = embedding_processor.Start(cancellationToken);

        It should_be_canceled = () => result.Status.ShouldEqual(TaskStatus.Canceled);
        It should_not_do_anything = () =>
        {
            state_updater.VerifyNoOtherCalls();
            event_waiter.VerifyNoOtherCalls();
            event_store.VerifyNoOtherCalls();
            embedding_store.VerifyNoOtherCalls();
            transition_calculator.VerifyNoOtherCalls();
        };
    }
}