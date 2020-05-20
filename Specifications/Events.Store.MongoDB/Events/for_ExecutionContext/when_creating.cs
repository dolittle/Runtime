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
            correlation = Guid.Parse("8d42c5d0-1753-4d88-9228-837ddc64d416");
            microservice = Guid.Parse("5586f689-407b-49cf-ac2e-fd12ceeccd83");
            tenant = Guid.Parse("5c9dfb13-8599-4211-8eec-9d5f3d808d1b");
            version = new Version(62007350, 808463667, 41937649, 1632080924, "Something very random");
            environment = "some very random environmen";
        };

        Because of = () => result = new ExecutionContext(
            correlation,
            microservice,
            tenant,
            version,
            environment);

        It should_have_the_correct_correlation = () => result.Correlation.ShouldEqual(correlation);
        It should_have_the_correct_microservice = () => result.Microservice.ShouldEqual(microservice);
        It should_have_the_correct_tenant = () => result.Tenant.ShouldEqual(tenant);
        It should_have_the_correct_version = () => result.Version.ShouldEqual(version);
        It should_have_the_correct_environment = () => result.Environment.ShouldEqual(environment);
    }
}
