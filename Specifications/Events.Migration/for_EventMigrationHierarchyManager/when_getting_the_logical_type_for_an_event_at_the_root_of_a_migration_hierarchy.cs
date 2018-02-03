using doLittle.Runtime.Events.Migration.Specs.Fakes;
using Machine.Specifications;
using System;

namespace doLittle.Runtime.Events.Migration.Specs.for_EventMigrationHierarchyManager
{
    public class when_getting_the_logical_type_for_an_event_at_the_root_of_a_migration_hierarchy : given.an_event_migration_hierarchy_manager_with_two_logical_events
    {
        static Type root_type;
        static Type logical_type;

        Because of = () =>
        {
            root_type = typeof(AnotherSimpleEvent);
            logical_type = event_migration_hierarchy_manager.GetLogicalTypeFor(root_type);
        };

        It should_return_the_type_passed_in_as_the_logical_type = () => logical_type.ShouldEqual(root_type);
    }
}