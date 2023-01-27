// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.when_trying_to_create.a_delete_request;

public class and_everything_works : given.all_dependencies
{

    static Try<Contracts.EmbeddingRequest> result;

    Because of = () => result = factory.TryCreate(current_state);

    It should_be_successful = () => result.Success.Should().BeTrue();
    It should_be_a_projection_request = () => result.Result.RequestCase.Should().Be(Contracts.EmbeddingRequest.RequestOneofCase.Delete);
    It should_have_the_correct_type = () => result.Result.Delete.ProjectionState.Type.Should().Be(current_state.Type.ToProtobuf());
    It should_have_the_correct_state = () => result.Result.Delete.ProjectionState.State.Should().Be(current_state.State.Value);
    It should_have_the_correct_key = () => result.Result.Delete.ProjectionState.Key.Should().Be(current_state.Key.Value);

}