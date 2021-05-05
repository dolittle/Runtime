// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionStates.when_comparing
{
    public class and_the_states_are_not_equal : given.all_dependencies
    {
        static ProjectionState left_state;
        static ProjectionState right_state;
        static CompareProjectionStates comparer;

        Establish context = () =>
        {
            left_state = new ProjectionState("equal_state");
            right_state = new ProjectionState("unequal_state");
            comparer = new CompareProjectionStates();
        };

        static Try<bool> result;

        Because of = () => result = comparer.TryCheckEquality(left_state, right_state);

        It should_fail = () => result.Success.ShouldBeFalse();
    }
}
