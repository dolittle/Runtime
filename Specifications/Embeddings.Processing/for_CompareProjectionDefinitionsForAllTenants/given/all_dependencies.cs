// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionDefinitionsForAllTenants.given;

public class all_dependencies
{
    protected static IExecutionContextManager execution_context_manager;
    protected static Func<IEmbeddingDefinitions> get_definitions;
    protected static ITenants tenants;
    protected static Dictionary<TenantId, Mock<IEmbeddingDefinitions>> embedding_definitions_per_tenant;

    Establish context = () =>
    {
        execution_context_manager = new ExecutionContextManager(Mock.Of<ILogger>());
        execution_context_manager.CurrentFor(new Execution.ExecutionContext(
            "826fd1b0-c620-4ce3-907e-f159e2f63e02",
            "63c756bb-6881-4b70-874a-ab1ae0e67d2b",
            Version.NotSet,
            "env",
            "d5d48a8c-e638-414d-92da-1b6862c8d0e2",
            Security.Claims.Empty,
            System.Globalization.CultureInfo.InvariantCulture));
        get_definitions = () =>
        {
            var tenant = execution_context_manager.Current.Tenant;
            return embedding_definitions_per_tenant[tenant].Object;
        };
    };

    protected static CompareEmbeddingDefinitionsForAllTenants WithTenants(Action<TenantConfigBuilder> callback)
    {
        var builder = new TenantConfigBuilder();
        callback(builder);
        builder.Build();
        return new CompareEmbeddingDefinitionsForAllTenants(new PerformActionOnAllTenants(tenants, execution_context_manager), get_definitions);
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
                new TenantsConfiguration(
                    definitionsPerTenant.ToDictionary(_ => _.Key.Value, _ => new TenantConfiguration())));
            embedding_definitions_per_tenant = new Dictionary<TenantId, Mock<IEmbeddingDefinitions>>();
            foreach (var (tenant, definitions) in definitionsPerTenant)
            {
                embedding_definitions_per_tenant.Add(tenant, definitions);
            }

        }
    }
}