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
    public class when_calling_begin_processing_events_after_events_have_been_queued : a_test_scoped_event_processing_hub
    {
        Establish context = () => commits.ForEach(c => hub.Process(c));

        Because of = () => hub.BeginProcessingEvents();

        It should_have_queued_all_the_event_streams = () => hub.Queued.Select(c => c.EventStream).ShouldContainOnly(commits);
        It should_process_all_the_event_streams = () => hub.Processed.Select(c => c.EventStream).ShouldContainOnly(commits);
        
    }
}