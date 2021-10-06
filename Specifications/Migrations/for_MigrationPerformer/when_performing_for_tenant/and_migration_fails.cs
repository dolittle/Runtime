// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
namespace Dolittle.Runtime.Migrations.for_MigrationPerformer.when_performing_for_tenant
{
    public class and_migration_fails : given.all_dependencies
    {
        static MigrationPerformer performer;
        static Mock<ICanMigrateAnEventStore> migration;
        static TenantId tenant;
        static Exception exception;
        static Try result;
        
        Establish context = () =>
        {
            exception = new Exception();
            migration = new Mock<ICanMigrateAnEventStore>();
            migration
                .Setup(_ => _.Migrate(event_store_configuration))
                .Returns(Task.FromResult(Try.Failed(exception)));
            tenant = "b19d9c7d-1433-4b25-a34a-8d6dc522835f";
            performer = new MigrationPerformer(
                resources_builder
                    .configure_tenant(tenant, _ => _.with_event_store(event_store_configuration)).build());
        };

        Because of = () => result = performer.PerformForTenant(migration.Object, tenant).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_with_the_right_exception = () => result.Exception.ShouldEqual(exception);
        It should_migrate = () => migration.Verify(_ => _.Migrate(event_store_configuration), Times.Once);
    }
}