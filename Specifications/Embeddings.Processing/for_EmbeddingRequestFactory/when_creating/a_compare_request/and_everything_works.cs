// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.when_creating.a_compare_request
{
    public class and_everything_works : given.all_dependencies
    {
        static UncommittedEvent @event;

        static Contracts.EmbeddingRequest result;

        Because of = () => result = factory.Create(current_state, desired_state);

        It should_be_a_projection_request = () => result.RequestCase.ShouldEqual(Contracts.EmbeddingRequest.RequestOneofCase.Compare);
        It should_have_the_correct_type = () => result.Compare.ProjectionState.Type.ShouldEqual(current_state.Type.ToProtobuf());
        It should_have_the_correct_state = () => result.Compare.ProjectionState.State.ShouldEqual(current_state.State.Value);
        It should_have_the_correct_key = () => result.Compare.ProjectionState.Key.ShouldEqual(current_state.Key.Value);
        It should_have_the_correct_desired_state = () => result.Compare.EntityState.ShouldEqual(desired_state.Value);

    }
}