// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionDefinitionsForAllTenants.given
{
    public class one_tenant : all_dependencies
    {
        protected static TenantId tenant;
        protected static Mock<IEmbeddingDefinitions> definitions;
        protected static CompareEmbeddingDefinitionsForAllTenants comparer;

        Establish context = () =>
        {
            tenant = "2471590e-caa2-4413-9abd-7f0478ad4191";
            definitions = new Mock<IEmbeddingDefinitions>();
            comparer = WithTenants(_ => _.ForTenant(tenant, definitions));
        };
    }
}