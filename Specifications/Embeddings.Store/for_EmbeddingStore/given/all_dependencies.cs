// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Embeddings.Store.State;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.Runtime.Embeddings.Store.for_EmbeddingStore.given
{
    public class all_dependencies
    {
        protected static Mock<IEmbeddingStates> states;
        protected static Mock<IEmbeddingDefinitions> definitions;
        protected static IEmbeddingStore store;

        Establish context = () =>
        {
            states = new Mock<IEmbeddingStates>();
            definitions = new Mock<IEmbeddingDefinitions>();
            store = new EmbeddingStore(states.Object, definitions.Object, new Mock<ILogger>().Object);
        };
    }
}
