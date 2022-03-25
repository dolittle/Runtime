// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.given;

public class all_dependencies
{
    protected static EmbeddingRequestFactory factory;

    Establish context = () =>
    {
        factory = new EmbeddingRequestFactory();
    };
}