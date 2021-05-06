// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingLoopDetector.when_detecting
{

    public class and_the_current_state_isnt_valid_json : given.all_dependencies
    {
        static ProjectionState current_state;

        Establish context = () =>
        {
            current_state = new ProjectionState("you 👉 wish i 🙋 was 👀👍 valid ✔✔ json haha 👌😂");
        };

        static Try<bool> result;

        Because of = () => result = detector.TryCheckForProjectionStateLoop(current_state, previous_states);

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_have_an_exception = () => result.HasException.ShouldBeTrue();
        It should_not_detect_a_loop = () => result.Result.ShouldBeFalse();
    }
}
