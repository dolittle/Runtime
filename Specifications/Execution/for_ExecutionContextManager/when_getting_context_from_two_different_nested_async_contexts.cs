// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Machine.Specifications;

namespace Dolittle.Runtime.Execution.for_ExecutionContextManager;

public class when_getting_context_from_two_different_nested_async_contexts : given.an_execution_context_manager
{
    static TenantId first_tenant;
    static TenantId second_tenant;
    static TenantId first_result;
    static TenantId second_result;

    Because of = () =>
    {
        first_tenant = Guid.NewGuid();
        second_tenant = Guid.NewGuid();
        var firstTask = Task.Run(() =>
        {
            execution_context_manager.CurrentFor(first_tenant, "", 0, "");

            Task.Run(() => first_result = new Guid(execution_context_manager.Current.Tenant.Value.ToByteArray())).Wait();
        });

        var secondTask = Task.Run(() =>
        {
            execution_context_manager.CurrentFor(second_tenant, "", 0, "");

            Task.Run(() => second_result = new Guid(execution_context_manager.Current.Tenant.Value.ToByteArray())).Wait();
        });

        Task.WaitAll(firstTask, secondTask);
    };

    It should_have_first_tenant_in_first_context = () => first_result.ShouldEqual(first_tenant);
    It should_have_second_tenant_in_second_context = () => second_result.ShouldEqual(second_tenant);
}