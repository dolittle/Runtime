using System;
using Machine.Specifications;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationHierarchyManager
{
    public class when_getting_the_concrete_type_for_an_unregistered_event : given.an_event_migration_hierarchy_manager_with_two_logical_events
    {
        static Exception exception;

        Because of = () =>
                         {
                             exception = Catch.Exception(() =>
                                    event_migration_hierarchy_manager.GetTargetTypeForGeneration(typeof(IEvent),1)
                                 );
                         };

        It should_throw_an_unregistered_event_exception = () => exception.ShouldBeOfExactType(typeof(UnregisteredEventException));
    }
}