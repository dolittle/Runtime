using System.Collections.ObjectModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Projections.Store.Definition;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Projections.for_CompareProjectionDefinitionsForAllTenants.given;

public class all_dependencies
{
    protected static CompareProjectionDefinitionsForAllTenants comparer;
    protected static Mock<ITenantServiceProviders> tenant_service_providers;
    protected static Mock<ITenants> tenants;
    protected static Mock<IProjectionDefinitions> definitions;
    Establish context = () =>
    {
        tenants = new Mock<ITenants>();
        definitions = new Mock<IProjectionDefinitions>();
        var services = new ServiceCollection();
        services.AddSingleton(definitions.Object);
        tenant_service_providers = new Mock<ITenantServiceProviders>();
        tenant_service_providers
            .Setup(_ => _.ForTenant(Moq.It.IsAny<TenantId>()))
            .Returns(services.BuildServiceProvider());
        tenants.Setup(_ => _.All).Returns(new ObservableCollection<TenantId>(new[] { TenantId.Development }));

        comparer = new CompareProjectionDefinitionsForAllTenants(new TenantActionPerformer(tenants.Object, tenant_service_providers.Object));
    };
}