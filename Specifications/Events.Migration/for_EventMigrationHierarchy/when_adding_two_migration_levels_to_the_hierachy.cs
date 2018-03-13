using Dolittle.Runtime.Events.Migration.Specs.Fakes.v2;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationHierarchy
{
    public class when_adding_two_migration_levels_to_the_hierachy : given.an_initialized_event_migration_hierarchy
    {
        Because of = () =>
                            {
                                event_migration_hierarchy.AddMigrationLevel(typeof(SimpleEvent));
                                event_migration_hierarchy.AddMigrationLevel(typeof(Fakes.v3.SimpleEvent));
                            };

        It should_have_still_have_the_logical_event_set_correctly = () => event_migration_hierarchy.LogicalEvent.ShouldEqual(hierarchy_for_type);
        It should_have_a_migration_level_of_two = () => event_migration_hierarchy.MigrationLevel.ShouldEqual(2);
    }
}