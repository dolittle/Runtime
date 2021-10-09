// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System.Threading.Tasks;
// using Dolittle.Runtime.ApplicationModel;
// using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
// using Dolittle.Runtime.Migrations;
// using Dolittle.Runtime.Migrations.for_MigrationPerformer.given;
// using Dolittle.Runtime.Rudimentary;
// using Machine.Specifications;
// using Moq;
// using It = Machine.Specifications.It;
// namespace Migrations.for_MigrationPerformer.when_performing_for_all_tenants
// {
//     public class and_everything_is_ok : all_dependencies
//     {
//         static MigrationPerformer performer;
//         static Mock<ICanMigrateAnEventStore> migration;
//         static TenantId first_tenant;
//         static TenantId second_tenant;
//         static Try result;
//         
//         Establish context = () =>
//         {
//             migration = new Mock<ICanMigrateAnEventStore>();migration
//                 .Setup(_ => _.Migrate(event_store_configuration))
//                 .Returns(Task.FromResult(Try.Succeeded()));
//             first_tenant = "b19d9c7d-1433-4b25-a34a-8d6dc522835f";
//             second_tenant = "664e446a-8a59-4da0-95d8-7138c12f3737";
//             performer = new MigrationPerformer(
//                 resources_builder
//                     .configure_tenant(first_tenant, _ => _.with_event_store(event_store_configuration))
//                     .configure_tenant(second_tenant, _ => _.with_event_store(event_store_configuration))
//                     .build());
//         };
//
//         Because of = () => result = performer.PerformForAllTenants(migration.Object).GetAwaiter().GetResult();
//
//         It should_not_fail = () => result.Success.ShouldBeTrue();
//         It should_migrate_twice = () => migration.Verify(_ => _.Migrate(event_store_configuration), Times.Exactly(2));
//     }
// }