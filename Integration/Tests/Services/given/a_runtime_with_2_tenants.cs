// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Domain.Tenancy;
using Integration.Shared;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Integration.Tests.Events.Processing.given;

class a_runtime_with_2_tenants
{
    protected static RunningRuntime runtime;
    protected static Dictionary<TenantId, ExecutionContext> execution_contexts;
    protected static TenantId[] tenants;

    Establish context = () =>
    {
        runtime = Runtime.CreateAndStart(2);
        tenants = runtime.ConfiguredTenants.ToArray();
        execution_contexts = tenants.ToDictionary(tenant => tenant, Runtime.CreateExecutionContextFor);
    };

    Cleanup after = () =>
    {
        Runtime.CleanAll(runtime).GetAwaiter().GetResult();
    };
}