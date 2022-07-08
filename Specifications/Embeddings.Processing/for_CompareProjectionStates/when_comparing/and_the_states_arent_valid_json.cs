// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionStates.when_comparing;

public class and_the_states_arent_valid_json
{
    static CompareProjectionStates comparer;
    static ProjectionState left;
    static ProjectionState right;

    Establish context = () =>
    {
        left = new ProjectionState("Non valid json");
        right = new ProjectionState("____");

        comparer = new CompareProjectionStates();
    };

    static Try<bool> result;

    Because of = () => result = comparer.TryCheckEquality(left, right);

    It should_fail = () => result.Success.ShouldBeFalse();
    It should_not_be_equal = () => result.Result.ShouldBeFalse();
}