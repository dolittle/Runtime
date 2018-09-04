namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.for_Process
{
    using Machine.Specifications;
    using Dolittle.Runtime.Events.Processing;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using Dolittle.Runtime.Events.Specs.Processing;
    using Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given;
    using Dolittle.Runtime.Events.Store;
    using System.Collections.Generic;
    using System.Linq;

    [Subject(typeof(ScopedEventProcessingHub),nameof(IScopedEventProcessingHub.Process))]
    public class when_processing_a_committed_event_stream_with_event_processors_registered : a_scoped_event_processor_hub_configured_with_processors
    {
        static CommittedEventStream committed_event_stream;
        static IEnumerable<CommittedEventEnvelope> committed_simple_events;
        static IEnumerable<CommittedEventEnvelope> committed_another_events;

        Establish context = () => 
        {
            hub.BeginProcessingEvents();
            committed_event_stream = specs.Events.Build();
            committed_simple_events = committed_event_stream.Events.Where(e => e.Metadata.Artifact == specs.Artifacts.artifact_for_simple_event).Select(e => e.ToCommittedEventEnvelope(committed_event_stream.Sequence));
            committed_another_events = committed_event_stream.Events.Where(e => e.Metadata.Artifact == specs.Artifacts.artifact_for_another_event).Select(e => e.ToCommittedEventEnvelope(committed_event_stream.Sequence));
        };
        Because of = () => hub.Process(committed_event_stream);

        It should_route_the_simple_events_to_the_simple_event_processor = () => simple_event_processor.Processed.ShouldContainOnly(committed_simple_events);
        It should_route_the_another_events_to_the_another_event_processor = () => another_event_processor.Processed.ShouldContainOnly(committed_another_events);
    }
}