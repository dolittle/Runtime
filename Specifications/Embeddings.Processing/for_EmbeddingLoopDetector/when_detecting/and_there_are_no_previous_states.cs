// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingLoopDetector.when_detecting;

public class and_there_are_no_previous_states : given.all_dependencies
{
    static ProjectionState current_state;

    Establish context = () =>
    {
        current_state = CreateStateWithKeyValue("ImAUniqueKey", "AndAUniqueValue");
        previous_states.Clear();
    };

    static Try<bool> result;

    Because of = () => result = detector.TryCheckForProjectionStateLoop(current_state, previous_states);

    It should_succeed = () => result.Success.Should().BeTrue();
    It should_not_detect_a_loop = () => result.Result.Should().BeFalse();

    It should_not_call_the_comparer = () =>
        comparer.Verify(_ =>
                _.TryCheckEquality(Moq.It.IsAny<ProjectionState>(), current_state),
            Times.Never());
}