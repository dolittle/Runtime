namespace Dolittle.Runtime.Processing.for_ScopedEventProcessor.when_processing
{
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using Machine.Specifications;
    using Moq;
    using It = Machine.Specifications.It;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using System.Linq;
    using System;
    using Dolittle.Runtime.Events.Specs.Processing;

    [Subject(typeof(ScopedEventProcessor),"Process")]
    public class and_the_processor_has_not_caught_up : scoped_event_processors
    {
        static CommittedEventVersion version = new CommittedEventVersion(1,1,0);
        static SingleEventTypeEventStream event_stream;
        static CommittedEventEnvelope event_to_process;

        Establish context = () => 
        {
            var event_artifact = specs.Artifacts.artifact_for_simple_event;
            var first_commit = specs.Events.Build(version);
            event_to_process = first_commit.Events.Where(e => e.Metadata.Artifact == event_artifact).Select(e => e.ToCommittedEventEnvelope(first_commit.Sequence)).Last();
            offset_repository_simple_tenant.Setup(r => r.Get(simple_scoped_processor.ProcessorId)).Returns(version);
        };
        Because of = () => simple_scoped_processor.Process(event_to_process);

        It should_have_the_version_set_as_none = () => simple_scoped_processor.LastVersionProcessed.ShouldEqual(CommittedEventVersion.None);
        It should_not_set_the_offset_with_the_repository = () => offset_repository_simple_tenant.Verify(r => r.Set(Moq.It.IsAny<EventProcessorId>(),Moq.It.IsAny<CommittedEventVersion>()),Times.Never());
        It should_not_have_passed_the_event_to_the_actual_event_processor_for_processing = () => simple_event_processor.ShouldNotHaveProcessedAnyEvents();
    }    
}