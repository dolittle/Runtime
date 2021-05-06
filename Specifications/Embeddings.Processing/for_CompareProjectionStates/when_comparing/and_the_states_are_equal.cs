// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionStates.when_comparing
{
    public class and_the_states_are_equal
    {
        static CompareProjectionStates comparer;
        static ProjectionState left;
        static ProjectionState right;

        Establish context = () =>
        {
            left = new ProjectionState(JsonConvert.SerializeObject(CreateState()));
            right = new ProjectionState(JsonConvert.SerializeObject(CreateState()));

            comparer = new CompareProjectionStates();
        };

        static Try<bool> result;
        Because of = () => result = comparer.TryCheckEquality(left, right);

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_be_equal = () => result.Result.ShouldBeTrue();

        static JObject CreateState()
        {
            dynamic state = new JObject();
            state.FirstProp = "FirstProp";
            state.Dictionary = new JObject
            {
                { "first_key", "first_value" },
                { "second_key", "second_value" },
                { "third_key", "third_value" }
            };
            state.Array = new JArray
            {
                "First",
                "Second",
                "Third"
            };
            return state;
        }
    }
}
