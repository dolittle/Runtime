using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.for_Process
{
    [Subject(typeof(ScopedEventProcessingHub),nameof(IScopedEventProcessingHub.Process))]
    public class when_processing_a_committed_event_stream_with_events_that_have_no_processors_registered : a_scoped_event_processor_hub_configured_with_some_processors
    {
        static CommittedEventStream committed_event_stream;
        static List<CommittedEventEnvelope> committed_simple_events;
        static List<CommittedEventEnvelope> committed_another_events;
        private static Exception ex;

        Establish context = () => 
        {
            hub.BeginProcessingEvents();
            committed_event_stream = Specs.given.Events.Build();
            committed_simple_events = committed_event_stream.Events.Where(e => e.Metadata.Artifact == Specs.given.Artifacts.artifact_for_simple_event).Select(e => e.ToCommittedEventEnvelope(committed_event_stream.Sequence)).ToList();
            committed_another_events = committed_event_stream.Events.Where(e => e.Metadata.Artifact == Specs.given.Artifacts.artifact_for_another_event).Select(e => e.ToCommittedEventEnvelope(committed_event_stream.Sequence)).ToList();
        };
        Because of = () => ex = Catch.Exception(() => hub.Process(committed_event_stream));

        It should_route_all_simple_events_to_the_simple_event_processor = () => simple_event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEventEnvelope>()),Moq.Times.Exactly(committed_simple_events.Count()));
        It should_route_each_simple_events_to_the_simple_event_processor = () => committed_simple_events.ForEach(_ => simple_event_processor.Verify(p => p.Process(_), Moq.Times.Once()));
        It should_ignore_the_events_with_no_processors = () => ex.ShouldBeNull();
    }
}