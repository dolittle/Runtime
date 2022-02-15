// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.when_trying_to_create.a_delete_request.given;

public class all_dependencies : for_EmbeddingRequestFactory.given.all_dependencies
{
    protected static EmbeddingCurrentState current_state;

    Establish context = () =>
    {
        current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.Persisted, "current state", "key");
    };
}