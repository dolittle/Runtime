// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.MongoDB;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using Dolittle.Runtime.Migrations;
using Dolittle.Runtime.Migrations.for_MigrationPerformer.given;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Specifications;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
namespace Migrations.for_MigrationPerformer.when_performing_for_all_tenants
{
    public class and_event_store_is_not_configured_for_tenant : all_dependencies
    {
        static MigrationPerformer performer;
        static Mock<ICanMigrateAnEventStore> migration;
        static TenantId tenant;
        static TenantId failing_tenant;
        static Try result;
        
        Establish context = () =>
        {
            migration = new Mock<ICanMigrateAnEventStore>();
            tenant = "b19d9c7d-1433-4b25-a34a-8d6dc522835f";
            failing_tenant = "664e446a-8a59-4da0-95d8-7138c12f3737";
            performer = new MigrationPerformer(
                resources_builder
                    .configure_tenant(tenant, _ => _.with_event_store(event_store_configuration))
                    .configure_tenant(failing_tenant, _ => {})
                    .build());
        };

        Because of = () => result = performer.PerformForAllTenants(migration.Object).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_because_event_store_is_not_configured_for_tenant = () => result.Exception.ShouldBeOfExactType<EventStoreNotConfiguredForTenant>();
        It should_not_migrate_anything = () => migration.Verify(_ => _.Migrate(Moq.It.IsAny<EventStoreConfiguration>()), Times.Never);
    }
}