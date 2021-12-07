// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_ExecutionContext;

public class when_converting_from_store_to_runtime_representation
{
    static ExecutionContext store_execution_context;
    static Dolittle.Runtime.Execution.ExecutionContext result;

    Establish context = () =>
    {
        store_execution_context = execution_contexts.create_store();
    };

    Because of = () => result = store_execution_context.ToExecutionContext();

    It should_have_the_correct_correlation = () => result.CorrelationId.Value.ShouldEqual(store_execution_context.Correlation);
    It should_have_the_correct_microservice = () => result.Microservice.Value.ShouldEqual(store_execution_context.Microservice);
    It should_have_the_correct_tenant = () => result.Tenant.Value.ShouldEqual(store_execution_context.Tenant);

    It should_have_the_correct_version = () =>
    {
        result.Version.Major.ShouldEqual(store_execution_context.Version.Major);
        result.Version.Minor.ShouldEqual(store_execution_context.Version.Minor);
        result.Version.Patch.ShouldEqual(store_execution_context.Version.Patch);
        result.Version.Build.ShouldEqual(store_execution_context.Version.Build);
        result.Version.PreReleaseString.ShouldEqual(store_execution_context.Version.PreRelease);
    };

    It should_have_the_correct_environment = () => result.Environment.Value.ShouldEqual(store_execution_context.Environment);
}