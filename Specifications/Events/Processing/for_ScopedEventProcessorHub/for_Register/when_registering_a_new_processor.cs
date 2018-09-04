namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.for_Register
{
    using System;
    using Dolittle.Artifacts;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Specs.Processing;
    using Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given;
    using Dolittle.Runtime.Tenancy;
    using Machine.Specifications;
    using Moq;
    using It = Machine.Specifications.It;

    [Subject(typeof(ScopedEventProcessingHub),nameof(IScopedEventProcessingHub.Register))]
    public class when_registering_a_new_processor : a_scoped_event_processor_hub
    {
        static Mock<IEventProcessor> processor;
        static Mock<ScopedEventProcessor> scoped_event_processor;
        static TenantId tenant;

        Establish context = () => 
        {
            tenant = Guid.NewGuid();
            processor = given.an_event_processor_mock();
            scoped_event_processor = given.a_scoped_event_processor_mock(tenant,processor.Object);
        };

        Because of = () => hub.Register(scoped_event_processor.Object);

        It should_register_the_event_processor_for_the_tenant_and_event = () => hub.GetProcessorsFor(scoped_event_processor.Object.Key).ShouldContainOnly(new []{scoped_event_processor.Object});
        It should_tell_the_processor_to_catch_up = () => scoped_event_processor.Verify(_ => _.CatchUp(), Times.Once());
    }
}