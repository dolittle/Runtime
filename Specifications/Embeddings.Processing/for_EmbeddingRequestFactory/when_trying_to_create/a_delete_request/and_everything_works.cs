// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using It = Machine.Specifications.It;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.when_trying_to_create.a_delete_request
{
    public class and_everything_works : given.all_dependencies
    {

        static Try<Contracts.EmbeddingRequest> result;

        Because of = () => result = factory.TryCreate(current_state);

        It should_be_successful = () => result.Success.ShouldBeTrue();
        It should_be_a_projection_request = () => result.Result.RequestCase.ShouldEqual(Contracts.EmbeddingRequest.RequestOneofCase.Delete);
        It should_have_the_correct_type = () => result.Result.Delete.ProjectionState.Type.ShouldEqual(current_state.Type.ToProtobuf());
        It should_have_the_correct_state = () => result.Result.Delete.ProjectionState.State.ShouldEqual(current_state.State.Value);
        It should_have_the_correct_key = () => result.Result.Delete.ProjectionState.Key.ShouldEqual(current_state.Key.Value);

    }
}
