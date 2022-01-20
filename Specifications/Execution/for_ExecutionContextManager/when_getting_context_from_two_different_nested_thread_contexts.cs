// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.ApplicationModel;
using Machine.Specifications;

namespace Dolittle.Runtime.Execution.for_ExecutionContextManager;

public class when_getting_context_from_two_different_nested_thread_contexts : given.an_execution_context_manager
{
    static TenantId first_tenant;
    static TenantId second_tenant;
    static TenantId first_result;
    static TenantId second_result;

    Because of = () =>
    {
        first_tenant = Guid.NewGuid();
        second_tenant = Guid.NewGuid();

        var first_thread = new Thread(() =>
        {
            execution_context_manager.CurrentFor(first_tenant, "", 0, "");
            var first_nested_thread = new Thread(() => first_result = new Guid(execution_context_manager.Current.Tenant.Value.ToByteArray()));
            first_nested_thread.Start();
            first_nested_thread.Join();
        });

        var second_thread = new Thread(() =>
        {
            execution_context_manager.CurrentFor(second_tenant, "", 0, "");
            var second_nested_thread = new Thread(() => second_result = new Guid(execution_context_manager.Current.Tenant.Value.ToByteArray()));
            second_nested_thread.Start();
            second_nested_thread.Join();
        });

        second_thread.Start();
        first_thread.Start();
        first_thread.Join();
        second_thread.Join();
    };

    It should_have_first_tenant_in_first_context = () => first_result.ShouldEqual(first_tenant);
    It should_have_second_tenant_in_second_context = () => second_result.ShouldEqual(second_tenant);
}