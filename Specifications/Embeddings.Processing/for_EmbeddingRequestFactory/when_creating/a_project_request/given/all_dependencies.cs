// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.when_creating.a_project_request.given;

public class all_dependencies : for_EmbeddingRequestFactory.given.all_dependencies
{
    protected static ProjectionCurrentState current_state;

    Establish context = () =>
    {
        current_state = new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, "current state", "key");
    };
}