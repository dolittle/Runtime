// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingLoopDetector.when_detecting;

public class and_a_previous_state_is_the_same : given.all_dependencies
{

    static ProjectionState current_state;

    Establish context = () =>
    {
        var (key, value) = ("Duplicate", "Value");
        current_state = CreateStateWithKeyValue(key, value);
        previous_states.Add(CreateStateWithKeyValue(key, value));

        comparer
            .Setup(_ => _.TryCheckEquality(Moq.It.IsAny<ProjectionState>(), current_state))
            .Returns(Try<bool>.Succeeded(true));
    };

    static Try<bool> result;

    Because of = () => result = detector.TryCheckForProjectionStateLoop(current_state, previous_states);

    It should_succeed = () => result.Success.Should().BeTrue();
    It should_detect_a_loop = () => result.Result.Should().BeTrue();
    It should_call_the_comparer = () =>
        comparer.Verify(_ =>
                _.TryCheckEquality(Moq.It.IsAny<ProjectionState>(), current_state),
            Times.AtLeastOnce());
}