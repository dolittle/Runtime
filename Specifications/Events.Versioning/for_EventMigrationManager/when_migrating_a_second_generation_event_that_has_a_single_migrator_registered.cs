using doLittle.Runtime.Events.Versioning.Specs.Fakes.v2;
using doLittle.Runtime.Events.Versioning.Specs.for_EventMigrationService.given;
using Machine.Specifications;
using doLittle.Events;

namespace doLittle.Runtime.Events.Versioning.Specs.for_EventMigrationService
{
    public class when_migrating_a_second_generation_event_that_has_a_single_migrator_registered : an_event_with_a_migrator
    {
        static IEvent result;

        Because of = () => result = event_migrator_manager.Migrate(source_event);

        It should_migrate_the_event_to_the_second_generation_type = () => result.ShouldBeOfExactType(typeof(SimpleEvent));
        It should_migrate_the_correct_values = () =>
                                                   {
                                                       var v2 = result as SimpleEvent;
                                                       v2.SecondGenerationProperty.ShouldEqual(SimpleEvent.DEFAULT_VALUE_FOR_SECOND_GENERATION_PROPERTY);
                                                   };
    }
}