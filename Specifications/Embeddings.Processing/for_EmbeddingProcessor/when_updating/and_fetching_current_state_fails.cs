// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_updating
{
    public class and_fetching_current_state_fails : given.all_dependencies_and_a_desired_state
    {
        static Task task;
        static Exception exception;

        Establish context = () =>
        {
            exception = new Exception();
            task = embedding_processor.Start(CancellationToken.None);
            embedding_store.Setup(_ => _.TryGet(embedding, key, CancellationToken.None)).Returns(Task.FromResult(Try<EmbeddingCurrentState>.Failed(exception)));
        };

        static Try<ProjectionState> result;

        Because of = () => result = embedding_processor.Update(key, desired_state, CancellationToken.None).GetAwaiter().GetResult();

        It should_still_be_running = () => task.Status.ShouldEqual(TaskStatus.Running);
        It should_return_the_failure = () => result.Exception.ShouldEqual(exception);
    }
}