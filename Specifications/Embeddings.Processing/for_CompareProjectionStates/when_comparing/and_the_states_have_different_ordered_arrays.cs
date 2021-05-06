// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionStates.when_comparing
{
    public class and_the_states_have_different_ordered_arrays
    {
        static CompareProjectionStates comparer;
        static ProjectionState left;
        static ProjectionState right;

        Establish context = () =>
        {
            var left_array = new JArray
            {
                "First",
                "Second",
                "Third"
            };
            dynamic left_dynamic = new JObject();
            left_dynamic.Array = left_array;
            left = new ProjectionState(JsonConvert.SerializeObject(left_dynamic));

            var right_array = new JArray
            {
                "Second",
                "Third",
                "First"
            };
            dynamic right_dynamic = new JObject();
            right_dynamic.Array = right_array;
            right = new ProjectionState(JsonConvert.SerializeObject(right_dynamic));

            comparer = new CompareProjectionStates();
        };

        static Try<bool> result;

        Because of = () => result = comparer.TryCheckEquality(left, right);

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_not_be_equal = () => result.Result.ShouldBeFalse();
    }
}
