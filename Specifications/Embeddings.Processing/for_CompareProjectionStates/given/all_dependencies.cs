// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionStates.given
{
    public class all_dependencies
    {
        protected static ProjectionState initial_state;

        Establish context = () =>
        {
            initial_state = "projection-initial-state";

        };
    }
}
