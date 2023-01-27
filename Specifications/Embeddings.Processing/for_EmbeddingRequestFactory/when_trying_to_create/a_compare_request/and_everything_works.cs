// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.when_trying_to_create.a_compare_request;

public class and_everything_works : given.all_dependencies
{

    static Try<Contracts.EmbeddingRequest> result;

    Because of = () => result = factory.TryCreate(current_state, desired_state);

    It should_be_successful = () => result.Success.Should().BeTrue();
    It should_be_a_projection_request = () => result.Result.RequestCase.Should().Be(Contracts.EmbeddingRequest.RequestOneofCase.Compare);
    It should_have_the_correct_type = () => result.Result.Compare.ProjectionState.Type.Should().Be(current_state.Type.ToProtobuf());
    It should_have_the_correct_state = () => result.Result.Compare.ProjectionState.State.Should().Be(current_state.State.Value);
    It should_have_the_correct_key = () => result.Result.Compare.ProjectionState.Key.Should().Be(current_state.Key.Value);
    It should_have_the_correct_desired_state = () => result.Result.Compare.EntityState.Should().Be(desired_state.Value);

}