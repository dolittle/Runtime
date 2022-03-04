// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Embeddings.Processing.for_PersistProjectionDefinitionForAllTenants.given;

public class all_dependencies
{
    protected static Func<TenantId, IEmbeddingDefinitions> get_definitions;
    protected static Mock<ITenantServiceProviders> tenant_service_providers;
    protected static ExecutionContext execution_context;
    protected static ITenants tenants;
    protected static Dictionary<TenantId, Mock<IEmbeddingDefinitions>> embedding_definitions_per_tenant;

    Establish context = () =>
    {
        tenant_service_providers = new Mock<ITenantServiceProviders>();
        execution_context = new ExecutionContext(
            "826fd1b0-c620-4ce3-907e-f159e2f63e02",
            "63c756bb-6881-4b70-874a-ab1ae0e67d2b",
            Version.NotSet,
            "env",
            "d5d48a8c-e638-414d-92da-1b6862c8d0e2",
            Claims.Empty,
            System.Globalization.CultureInfo.InvariantCulture);
        get_definitions = tenant => embedding_definitions_per_tenant[tenant].Object;
    };

    protected static PersistEmbeddingDefinitionForAllTenants WithTenants(Action<TenantConfigBuilder> callback)
    {
        var builder = new TenantConfigBuilder();
        callback(builder);
        builder.Build();
        return new PersistEmbeddingDefinitionForAllTenants(new TenantActionPerformer(tenants, tenant_service_providers.Object), get_definitions, Mock.Of<ILogger>());
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