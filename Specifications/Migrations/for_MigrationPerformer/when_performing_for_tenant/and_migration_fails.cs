// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.MongoDB;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
namespace Dolittle.Runtime.Migrations.for_MigrationPerformer.when_performing_for_tenant
{
    public class and_migration_fails : given.all_dependencies
    {
        static MigrationPerformer performer;
        static TenantId tenant;
        static Exception exception;
        static Try result;
        
        Establish context = () =>
        {
            exception = new Exception();
            migration
                .Setup(_ => _.Migrate(Moq.It.IsAny<EventStoreConfiguration>()))
                .Returns(Task.FromResult(Try.Failed(exception)));
            tenant = "b19d9c7d-1433-4b25-a34a-8d6dc522835f";
            performer = create_performer(_ =>
                _.configure_tenant(tenant, _ => _.with_event_store()));
        };

        Because of = () => result = performer.PerformForTenant(migration.Object, tenant).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_with_the_right_exception = () => result.Exception.ShouldEqual(exception);
        It should_migrate = () => migration.Verify(_ => _.Migrate(Moq.It.IsAny<EventStoreConfiguration>()), Times.Once);
    }
}