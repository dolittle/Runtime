// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingLoopDetector.when_detecting
{

    public class and_the_current_state_isnt_valid_json : given.all_dependencies
    {
        static ProjectionState current_state;
        static Exception exception;

        Establish context = () =>
        {
            current_state = new ProjectionState("this 👈 text doesn't actually matter 🤛🙅 bcus we just 📲👌 setup the 😏💲 comparer to 💦 fail ☠🤧 haha 😎😂");
            exception = new Exception();
            comparer
                .Setup(_ => _.TryCheckEquality(Moq.It.IsAny<ProjectionState>(), current_state))
                .Returns(exception);
        };

        static Try<bool> result;

        Because of = () => result = detector.TryCheckForProjectionStateLoop(current_state, previous_states);

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_have_the_correct_exception = () => result.Exception.ShouldEqual(exception);
        It should_call_the_comparer = () =>
            comparer.Verify(_ =>
                _.TryCheckEquality(Moq.It.IsAny<ProjectionState>(), current_state),
                Times.AtLeastOnce());
    }
}
