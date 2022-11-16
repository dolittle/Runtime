// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Domain.Tenancy;
using Integration.Shared;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Integration.Tests.Events.Store.given;

[Tags("IntegrationTest")]
public class a_runtime_with_a_single_tenant
{
    protected static RunningRuntime runtime;
    protected static ExecutionContext execution_context;
    protected static TenantId tenant;
    
    Establish context = () =>
    {
        runtime = Runtime.CreateAndStart(1);
        tenant = runtime.ConfiguredTenants.First();
        execution_context = Runtime.CreateExecutionContextFor(tenant);
    };

    Cleanup mess = () =>
    {
        Runtime.CleanAll(runtime).GetAwaiter().GetResult();
    };
}