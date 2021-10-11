// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Migrations;
using Dolittle.Runtime.Migrations.for_MigrationPerformer.given;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;
namespace Migrations.for_MigrationPerformer.when_performing_for_all_tenants
{
    public class and_no_tenants_are_configured : all_dependencies
    {
        static MigrationPerformer performer;
        static Try result;
        
        Establish context = () =>
        {
            performer = create_performer(_ => {});
        };

        Because of = () => result = performer.PerformForAllTenants(migration.Object).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_because_tenant_is_not_configured = () => result.Exception.ShouldBeOfExactType<NoTenantsConfigured>();
    }
}