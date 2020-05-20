// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_ExecutionContext
{
    public class when_creating
    {
        static Guid correlation;
        static Guid microservice;
        static Guid tenant;
        static Version version;
        static string environment;
        static ExecutionContext result;

        Establish context = () =>
        {
            correlation = Guid.NewGuid();
            microservice = Guid.NewGuid();
            tenant = Guid.NewGuid();
            version = new Version(0, 0, 1, 0, "");
            environment = "some environmen";
        };

        Because of = () => result = new ExecutionContext(
            correlation,
            microservice,
            tenant,
            version,
            environment,
            );

        It should_have_the_correct_correlation = () => result.Correlation.ShouldEqual(correlation);
        It should_have_the_correct_microservice = () => result.Microservice.ShouldEqual(microservice);
        It should_have_the_correct_tenant = () => result.Tenant.ShouldEqual(tenant);
        It should_have_the_correct_version = () => result.Version.ShouldEqual(version);
        It should_have_the_correct_environment = () => result.Environment.ShouldEqual(environment);
    }
}
