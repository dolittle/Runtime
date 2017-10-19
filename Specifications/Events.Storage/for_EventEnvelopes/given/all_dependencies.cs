using doLittle.Runtime.Events.Versioning;
using doLittle.Runtime.Applications;
using doLittle.Runtime.Execution;
using doLittle.Time;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Events.Storage.Specs.for_EventEnvelopes.given
{
    public class all_dependencies
    {
        protected static Mock<IApplicationResources> application_resources;
        protected static Mock<ISystemClock> system_clock;
        protected static Mock<IExecutionContext> execution_context;
        protected static Mock<IEventMigrationHierarchyManager> event_migration_hierarchy_manager;

        Establish context = () =>
        {
            application_resources = new Mock<IApplicationResources>();
            system_clock = new Mock<ISystemClock>();
            execution_context = new Mock<IExecutionContext>();
            event_migration_hierarchy_manager = new Mock<IEventMigrationHierarchyManager>();
        };
    }
}
