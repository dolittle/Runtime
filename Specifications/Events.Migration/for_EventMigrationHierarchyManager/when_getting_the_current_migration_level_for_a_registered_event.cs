using Machine.Specifications;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationHierarchyManager
{
    public class when_getting_the_current_migration_level_for_a_registered_event : given.an_event_migration_hierarchy_manager_with_two_logical_events
    {
        static Generation level_for_event_without_migrations;
        static Generation level_for_event_with_migrations;

        Because of = () =>
                         {
                             level_for_event_without_migrations = event_migration_hierarchy_manager.GetCurrentGenerationFor(event_without_migrations);
                             level_for_event_with_migrations = event_migration_hierarchy_manager.GetCurrentGenerationFor(event_with_migrations);
                         };

        It should_get_the_correct_migration_level_for_a_migrated_event = () => level_for_event_with_migrations.Value.ShouldEqual(2u);
        It should_get_the_correct_migration_level_for_an_unmigrated_event = () => level_for_event_without_migrations.Value.ShouldEqual(0u);
    }
}