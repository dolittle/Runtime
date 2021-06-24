// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessors.given
{
    public class two_tenants_and_processors : two_tenants
    {
        protected static Mock<CreateEmbeddingProcessorForTenant> factory;
        protected static EmbeddingProcessors processors;

        Establish context = () =>
        {
            factory = new Mock<CreateEmbeddingProcessorForTenant>();
            processors = new EmbeddingProcessors(tenants.Object, Mock.Of<ILogger>());
        };
    }
}