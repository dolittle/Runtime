using doLittle.Runtime.Events.Versioning.Specs.Fakes;
using doLittle.Runtime.Events.Versioning.Specs.for_EventMigrationManager.given;
using Machine.Specifications;

namespace doLittle.Runtime.Events.Versioning.Specs.for_EventMigrationService.given
{
    public abstract class an_event_with_a_migrator : an_event_migrator_service_with_no_registered_migrators
    {
        Establish context = () => event_migrator_manager.RegisterMigrator(typeof(SimpleEventV1ToV2Migrator));
    }
}