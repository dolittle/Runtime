// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingLoopDetector.when_detecting;

public class and_previous_states_are_different : given.all_dependencies
{
    static ProjectionState current_state;

    Establish context = () =>
    {
        current_state = CreateStateWithKeyValue("ImAUniqueKey", "AndAUniqueValue");
        comparer
            .Setup(_ => _.TryCheckEquality(Moq.It.IsAny<ProjectionState>(), current_state))
            .Returns(Try<bool>.Succeeded(false));
    };

    static Try<bool> result;

    Because of = () => result = detector.TryCheckForProjectionStateLoop(current_state, previous_states);

    It should_succeed = () => result.Success.Should().BeTrue();
    It should_not_detect_a_loop = () => result.Result.Should().BeFalse();
    It should_call_the_comparer_on_the_whole_list = () =>
        comparer.Verify(_ =>
                _.TryCheckEquality(Moq.It.IsAny<ProjectionState>(), current_state),
            Times.Exactly(previous_states.Count));
}