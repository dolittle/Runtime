// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Versioning;
using Machine.Specifications;
namespace Dolittle.Runtime.Migrations.for_Migrations.when_getting_migration
{
    public class and_there_are_no_migrations : given.all_dependencies
    {
        static Migrations migrations;
        static Version to;
        static Version from;
        static Try<ICanMigrateDataStores> result;
        Establish context = () =>
        {
            migrations = given.all_dependencies.get_migrations();
            to = Version.NotSet;
            from = Version.NotSet;
        };

        Because of = () => result = migrations.GetFor(from, to);

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_because_no_migrator_defined = () => result.Exception.ShouldBeOfExactType<NoMigratorDefinedBetweenVersions>();
    }
}