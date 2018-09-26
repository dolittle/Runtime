namespace Dolittle.Runtime.Events.Specs.Processing.for_BootProcessing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Dolittle.Artifacts;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Types;
    using Dolittle.Collections;
    using Machine.Specifications;
    using Moq;
    using It = Machine.Specifications.It;
    using Dolittle.Runtime.Events.Specs;
    using Dolittle.Tenancy;
    using Dolittle.Runtime.Tenancy;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Specs.Processing;
    using Dolittle.Execution;

    [Subject(typeof(BootProcedure), nameof(BootProcedure.Perform))]
    public class when_performing_the_boot_procedure
    {
        static BootProcedure boot_procedure;
        static Mock<IInstancesOf<IKnowAboutEventProcessors>> I_know_about_event_processors;
        static Mock<ITenants> tenants;
        static Mock<IScopedEventProcessingHub> processing_hub;
        static Exception exception;
        static int number_of_scoped_processors;
        static int number_of_processors_per_tenant;
        static Mock<IFetchUnprocessedEvents> unprocessed_events_fetcher = new Mock<IFetchUnprocessedEvents>();
        static Mock<IEventProcessorOffsetRepository> offset_repository = new Mock<IEventProcessorOffsetRepository>();
        static Mock<IExecutionContextManager> execution_context_manager;

        Establish context = () =>
        {
            execution_context_manager = new Mock<IExecutionContextManager>();
            unprocessed_events_fetcher = new Mock<IFetchUnprocessedEvents>();
            offset_repository = new Mock<IEventProcessorOffsetRepository>();

            I_know_about_event_processors = get_instances_of_I_know_about_event_processors();
            I_know_about_event_processors.Object.ForEach(_ => number_of_processors_per_tenant += _.Count()); 
            tenants = get_tenants();
            number_of_scoped_processors = tenants.Object.All.Count() * number_of_processors_per_tenant;
            processing_hub = new Mock<IScopedEventProcessingHub>();
            boot_procedure = new BootProcedure(I_know_about_event_processors.Object,
                                                tenants.Object,processing_hub.Object, 
                                                () => offset_repository.Object,
                                                () => unprocessed_events_fetcher.Object,
                                                execution_context_manager.Object,
                                                mocks.a_logger().Object);
        };

        Because of = () => exception = Catch.Exception(() => boot_procedure.Perform());

        It should_not_have_any_errors = () => exception.ShouldBeNull();
        It should_register_a_scoped_event_processor_for_each_processor_tenant_combination = () => processing_hub.Verify(_ => _.Register(Moq.It.IsAny<ScopedEventProcessor>()),Times.Exactly(number_of_scoped_processors));

        It should_set_the_event_context_for_each_processor_for_each_tenant = () => tenants.Object.All.ForEach(t => execution_context_manager.Verify(_ => _.CurrentFor(t,"",0,""),Times.Exactly(number_of_processors_per_tenant)));
        It should_tell_the_hub_to_begin_processing = () => processing_hub.Verify(_ => _.BeginProcessingEvents(),Times.Once()); 

        static Mock<ITenants> get_tenants()
        {
            var list = new List<TenantId>();
            list.AddRange(Enumerable.Range(0, 10).Select(_ => (TenantId)Guid.NewGuid()));
            var tenants = new Mock<ITenants>();
            tenants.SetupGet(_ => _.All).Returns(() => list);
            return tenants;
        }

        static Mock<IInstancesOf<IKnowAboutEventProcessors>> get_instances_of_I_know_about_event_processors()
        {
            var know_about_processors = new Mock<IInstancesOf<IKnowAboutEventProcessors>>();
            var processors = new List<IKnowAboutEventProcessors>();
            processors.AddRange(Enumerable.Range(0, 3).Select(_ => get_instance_of_I_know_about_event_processor().Object));
            know_about_processors.Setup(_ => _.GetEnumerator()).Returns(() => processors.GetEnumerator());
            return know_about_processors;
        }

        static Mock<IKnowAboutEventProcessors> get_instance_of_I_know_about_event_processor()
        {
            var I_know = new Mock<IKnowAboutEventProcessors>();
            var event_processors = new List<IEventProcessor>();
            event_processors.AddRange(Enumerable.Range(0, 10).Select(_ => given.an_event_processor_mock(Artifact.New()).Object));
            I_know.Setup(_ => _.GetEnumerator()).Returns(() => event_processors.GetEnumerator());
            return I_know;
        }
    }
}