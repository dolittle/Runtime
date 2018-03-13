using System;
using Machine.Specifications;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationHierarchy
{
    public class when_adding_a_migration_that_does_not_implement_the_migration_interface : given.an_initialized_event_migration_hierarchy
    {
        static Exception Exception;

        Because of = () =>
        {
            Exception = Catch.Exception(() => event_migration_hierarchy.AddMigrationLevel(typeof(IEvent)));
        };

        It should_throw_a_not_a_migrated_event_type_exception = () => Exception.ShouldBeOfExactType(typeof(NotAMigratedEventTypeException));
    }
}