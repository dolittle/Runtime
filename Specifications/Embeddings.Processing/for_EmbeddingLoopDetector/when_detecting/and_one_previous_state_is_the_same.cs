// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingLoopDetector.when_detecting
{

    public class and_one_previous_state_is_the_same : given.all_dependencies
    {

        static ProjectionState current_state;

        Establish context = () =>
        {
            var (key, value) = ("Duplicate", "Value");
            current_state = CreateStateWithKeyValue(key, value);
            previous_states.Add(CreateStateWithKeyValue(key, value));
        };

        static Try<bool> result;

        Because of = () => result = detector.TryCheckForProjectionStateLoop(current_state, previous_states);

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_detect_a_loop = () => result.Result.ShouldBeTrue();
    }
}
