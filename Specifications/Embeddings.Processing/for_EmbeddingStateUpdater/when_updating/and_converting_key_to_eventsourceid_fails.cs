// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingStateUpdater.when_updating
{
    public class and_converting_key_to_eventsourceid_fails : given.all_dependencies
    {
        static ProjectionKey projection_key;
        static EmbeddingCurrentState current_state;
        static Exception exception;

        Establish context = () =>
        {
            projection_key = "projection-key";
            current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "state-current", projection_key);
            exception = new Exception();

            embedding_store
                .Setup(_ => _.TryGetKeys(embedding, cancellation_token))
                .Returns(Task.FromResult<Try<IEnumerable<ProjectionKey>>>(new[] { projection_key }));
            embedding_store
                .Setup(_ => _.TryGet(embedding, projection_key, cancellation_token))
                .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state));
            key_converter.Setup(_ => _.GetEventSourceIdFor(projection_key)).Throws(exception);
        };

        static Try result;
        Because of = () => result = state_updater.TryUpdateAll(cancellation_token).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_with_the_correct_error = () => result.Exception.ShouldBeTheSameAs(exception);
        It should_ask_the_embedding_store_for_keys = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token));
    }
}
