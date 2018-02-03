using doLittle.Execution;
using doLittle.Runtime.Events.Migration.Specs.Fakes;
using Machine.Specifications;
using Moq;
using System;
using doLittle.Types;
using doLittle.DependencyInversion;

namespace doLittle.Runtime.Events.Migration.Specs.for_EventMigrationManager.given
{
    public abstract class an_event_migrator_service_with_no_registered_migrators
    {
        protected static SimpleEvent source_event;
        protected static Guid event_source_id;
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;
        protected static EventMigratorManager event_migrator_manager;

        Establish context = () =>
                                {
                                    event_source_id = Guid.NewGuid();
                                    source_event = new SimpleEvent();
                                    type_finder = new Mock<ITypeFinder>();
                                    container = new Mock<IContainer>();
                                    container.Setup(c => c.Get(Moq.It.IsAny<Type>())).Returns(
                                        (Type t) => Activator.CreateInstance(t));

                                    event_migrator_manager = new EventMigratorManager(type_finder.Object, container.Object);
                                };
    }
}
