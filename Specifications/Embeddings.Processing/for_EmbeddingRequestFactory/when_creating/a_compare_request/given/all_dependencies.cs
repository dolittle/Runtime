// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.when_creating.a_compare_request.given;

public class all_dependencies : for_EmbeddingRequestFactory.given.all_dependencies
{
    protected static EmbeddingCurrentState current_state;
    protected static ProjectionState desired_state;

    Establish context = () =>
    {
        current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.Persisted, "current state", "key");
        desired_state = "desired state";
    };
}