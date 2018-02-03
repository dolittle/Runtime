using System;
using doLittle.Runtime.Events.Migration.Specs.Fakes.v3;
using Machine.Specifications;

namespace doLittle.Runtime.Events.Migration.Specs.for_EventMigrationHierarchyManager
{
    public class when_getting_the_logical_type_for_an_event_within_a_migration_hierarchy : given.an_event_migration_hierarchy_manager_with_two_logical_events
    {
        static Type logical_type;

        Because of = () =>
                         {
                             logical_type = event_migration_hierarchy_manager.GetLogicalTypeFor(typeof(SimpleEvent));
                         };

        It should_return_the_correct_logical_type_for_the_event_hierachy = () => logical_type.ShouldEqual(typeof(Fakes.SimpleEvent));
    }
}