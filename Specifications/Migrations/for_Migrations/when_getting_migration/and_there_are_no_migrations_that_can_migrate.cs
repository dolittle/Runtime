// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Versioning;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
namespace Dolittle.Runtime.Migrations.for_Migrations.when_getting_migration
{
    public class and_there_are_no_migrations_that_can_migrate : given.all_dependencies
    {
        static Migrations migrations;
        static Version to;
        static Version from;
        static Mock<ICanMigrateDataStores> first_migration;
        static Mock<ICanMigrateDataStores> second_migration;
        static Try<ICanMigrateDataStores> result;
        Establish context = () =>
        {
            first_migration = new Mock<ICanMigrateDataStores>();
            second_migration = new Mock<ICanMigrateDataStores>();
            to = Version.NotSet;
            from = Version.NotSet;

            first_migration.Setup(_ => _.CanMigrateFor(from, to)).Returns(false);
            second_migration.Setup(_ => _.CanMigrateFor(from, to)).Returns(false);
            add_migrations(first_migration.Object, second_migration.Object);
            migrations = get_migrations();
        };

        Because of = () => result = migrations.GetFor(from, to);

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_because_no_migrator_defined = () => result.Exception.ShouldBeOfExactType<NoMigratorDefinedBetweenVersions>();
        It should_check_if_first_migration_can_migrate = () => first_migration.Verify(_ => _.CanMigrateFor(from, to));
        It should_check_if_second_migration_can_migrate = () => second_migration.Verify(_ => _.CanMigrateFor(from, to));
    }
}