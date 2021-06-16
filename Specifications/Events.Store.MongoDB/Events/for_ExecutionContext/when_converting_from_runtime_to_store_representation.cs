// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_ExecutionContext
{
    public class when_converting_from_runtime_to_store_representation
    {
        static Dolittle.Runtime.Execution.ExecutionContext runtime_execution_context;
        static ExecutionContext result;

        Establish context = () =>
        {
            runtime_execution_context = execution_contexts.create();
        };

        Because of = () => result = runtime_execution_context.ToStoreRepresentation();

        It should_have_the_correct_correlation = () => result.Correlation.ShouldEqual(runtime_execution_context.CorrelationId.Value);
        It should_have_the_correct_microservice = () => result.Microservice.ShouldEqual(runtime_execution_context.Microservice.Value);
        It should_have_the_correct_tenant = () => result.Tenant.ShouldEqual(runtime_execution_context.Tenant.Value);

        It should_have_the_correct_version = () =>
        {
            result.Version.Major.ShouldEqual(runtime_execution_context.Version.Major);
            result.Version.Minor.ShouldEqual(runtime_execution_context.Version.Minor);
            result.Version.Patch.ShouldEqual(runtime_execution_context.Version.Patch);
            result.Version.Build.ShouldEqual(runtime_execution_context.Version.Build);
            result.Version.PreRelease.ShouldEqual(runtime_execution_context.Version.PreReleaseString);
        };

        It should_have_the_correct_environment = () => result.Environment.ShouldEqual(runtime_execution_context.Environment.Value);
    }
}
