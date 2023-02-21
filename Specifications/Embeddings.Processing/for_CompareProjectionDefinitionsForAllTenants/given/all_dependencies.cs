// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Microsoft.Extensions.Options;
using Moq;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionDefinitionsForAllTenants.given;

public class all_dependencies
{
    protected static Func<TenantId, IEmbeddingDefinitions> get_definitions;
    protected static ITenants tenants;
    protected static Dictionary<TenantId, Mock<IEmbeddingDefinitions>> embedding_definitions_per_tenant;
    protected static Mock<ITenantServiceProviders> service_providers;

    Establish context = () =>
    {
        service_providers = new Mock<ITenantServiceProviders>();
        get_definitions = tenant => embedding_definitions_per_tenant[tenant].Object;
    };

    protected static CompareEmbeddingDefinitionsForAllTenants WithTenants(Action<TenantConfigBuilder> callback)
    {
        var builder = new TenantConfigBuilder();
        callback(builder);
        builder.Build();
        return new CompareEmbeddingDefinitionsForAllTenants(new TenantActionPerformer(tenants, service_providers.Object), get_definitions);
    }

    public class TenantConfigBuilder
    {
        readonly Dictionary<TenantId, Mock<IEmbeddingDefinitions>> definitionsPerTenant = new();

        public TenantConfigBuilder ForTenant(TenantId tenant, Mock<IEmbeddingDefinitions> definitions)
        {
            definitionsPerTenant.Add(tenant, definitions);
            return this;
        }

        public void Build()
        {
            tenants = new Tenants(
                new OptionsWrapper<TenantsConfiguration>(new TenantsConfiguration(
                    definitionsPerTenant.ToDictionary(_ => _.Key, _ => new TenantConfiguration()))));
            embedding_definitions_per_tenant = new Dictionary<TenantId, Mock<IEmbeddingDefinitions>>();
            foreach (var (tenant, definitions) in definitionsPerTenant)
            {
                embedding_definitions_per_tenant.Add(tenant, definitions);
            }
        }
    }
}