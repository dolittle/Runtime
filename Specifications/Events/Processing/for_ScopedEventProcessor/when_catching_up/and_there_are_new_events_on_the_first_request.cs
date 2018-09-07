namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessor.when_catching_up
{
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using Machine.Specifications;
    using Moq;
    using It = Machine.Specifications.It;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using System.Linq;
    using System;

    [Subject(typeof(ScopedEventProcessor),"CatchUp")]
    public class and_there_are_new_events_on_the_first_request : scoped_event_processors
    {
        static CommittedEventVersion version = new CommittedEventVersion(1,1,1);
        static CommittedEventVersion expected_version;

        Establish context = () => 
        {
            var event_artifact = specs.Artifacts.artifact_for_simple_event;
            var first_commit = specs.Events.Build(version);
            var last_stream = specs.Events.Build(first_commit.LastEventVersion);

            var events = first_commit.Events.Where(e => e.Metadata.Artifact == event_artifact).Select(e => e.ToCommittedEventEnvelope(first_commit.Sequence)).ToList();
            events.AddRange(last_stream.Events.Where(e => e.Metadata.Artifact == event_artifact).Select(e => e.ToCommittedEventEnvelope(last_stream.Sequence)));

            expected_version = events.Last().Version;
            offset_repository_simple_tenant.Setup(r => r.Get(simple_scoped_processor.ProcessorId)).Returns(version);
            unprocessed_events_fetcher_for_tenant_simple.Setup(f => f.GetUnprocessedEvents(event_artifact.Id,version)).Returns(new SingleEventTypeEventStream(events));
            unprocessed_events_fetcher_for_tenant_simple.Setup(f => f.GetUnprocessedEvents(event_artifact.Id,last_stream.LastEventVersion)).Returns(new SingleEventTypeEventStream(null));
        };
        Because of = () => simple_scoped_processor.CatchUp();

        It should_retrieve_the_offset_from_the_repository = () => offset_repository_simple_tenant.Verify(r => r.Get(simple_scoped_processor.ProcessorId), Times.Once());
        It should_retrieve_events_since_the_retrieved_last_processed_version = () => unprocessed_events_fetcher_for_tenant_simple.Verify(f => f.GetUnprocessedEvents(specs.Artifacts.artifact_for_simple_event.Id,version), Times.Once());
        It should_retrieve_events_since_the_events_it_has_processed = () => unprocessed_events_fetcher_for_tenant_simple.Verify(f => f.GetUnprocessedEvents(specs.Artifacts.artifact_for_simple_event.Id,expected_version), Times.Once());
        It should_have_the_version_set_from_the_last_event_it_processed = () => simple_scoped_processor.LastVersionProcessed.ShouldEqual(expected_version);
    }
}