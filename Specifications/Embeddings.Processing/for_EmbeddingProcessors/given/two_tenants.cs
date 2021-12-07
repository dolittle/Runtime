// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessors.given;

public class two_tenants
{
    protected static TenantId tenant_a;
    protected static TenantId tenant_b;

    static ObservableCollection<TenantId> tenantIds;
    protected static Mock<ITenants> tenants;

    Establish context = () =>
    {
        tenant_a = "a9804f28-e9b2-480e-95f9-e0163d016d57";
        tenant_b = "32467aff-ab40-4c0c-b5a1-0ee91035ad33";

        tenantIds = new ObservableCollection<TenantId>();
        tenantIds.Add(tenant_a);
        tenantIds.Add(tenant_b);

        tenants = new Mock<ITenants>();
        tenants.Setup(_ => _.All).Returns(tenantIds);
    };
}