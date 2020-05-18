// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class ShouldExtensions
    {
        public static void ShouldBeTheSameAs(this ExecutionContext storedExecutionContext, Execution.ExecutionContext executionContext)
        {
            storedExecutionContext.Correlation.ShouldEqual(executionContext.CorrelationId.Value);
            storedExecutionContext.Environment.ShouldEqual(executionContext.Environment.Value);
            storedExecutionContext.Microservice.ShouldEqual(executionContext.Microservice.Value);
            storedExecutionContext.Tenant.ShouldEqual(executionContext.Tenant.Value);
            storedExecutionContext.Version.ShouldBeTheSameAs(executionContext.Version);
        }

        public static void ShouldBeTheSameAs(this MongoDB.Events.Version storedVersion, Dolittle.Versioning.Version version)
        {
            storedVersion.Major.ShouldEqual(version.Major);
            storedVersion.Minor.ShouldEqual(version.Minor);
            storedVersion.Patch.ShouldEqual(version.Patch);
            storedVersion.Build.ShouldEqual(version.Build);
            storedVersion.PreRelease.ShouldEqual(version.PreReleaseString);
        }

        public static void ShouldBeTheSameAs(this Execution.ExecutionContext executionContext, ExecutionContext storedExecutionContext)
        {
            executionContext.CorrelationId.Value.ShouldEqual(storedExecutionContext.Correlation);
            executionContext.Environment.Value.ShouldEqual(storedExecutionContext.Environment);
            executionContext.Microservice.Value.ShouldEqual(storedExecutionContext.Microservice);
            executionContext.Tenant.Value.ShouldEqual(storedExecutionContext.Tenant);
            executionContext.Version.ShouldBeTheSameAs(storedExecutionContext.Version);
        }

        public static void ShouldBeTheSameAs(this Dolittle.Versioning.Version version, MongoDB.Events.Version storedVersion)
        {
            version.Major.ShouldEqual(storedVersion.Major);
            version.Minor.ShouldEqual(storedVersion.Minor);
            version.Patch.ShouldEqual(storedVersion.Patch);
            version.Build.ShouldEqual(storedVersion.Build);
            version.PreReleaseString.ShouldEqual(storedVersion.PreRelease);
        }
    }
}
