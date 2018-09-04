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
    public class when_processing_a_stream_after_begin_processing_events_has_been_called : a_test_scoped_event_processing_hub
    {
        Establish context = () => hub.BeginProcessingEvents();

        Because of = () => commits.ForEach(c => hub.Process(c));

        It should_process_all_the_event_streams = () => hub.Processed.Select(c => c.EventStream).ShouldContainOnly(commits);
        It should_not_queue_any_event_streams = () => hub.Queued.Any().ShouldBeFalse();
    }
}