using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_EventSource.given
{
    [Subject(typeof(EventSourceExtensions))]
    public class two_different_event_source_types_that_handle_different_events
    {
        protected static StatefulEventSource event_source;
        protected static AnotherStatefulEventSource second_event_source;
        protected static Guid event_source_id;
        protected static Guid second_event_source_id;
        protected static SimpleEvent simple_event;
        protected static AnotherSimpleEvent another_simple_event;
        
        Establish context = () =>
                                {
                                    event_source_id = Guid.NewGuid();
                                    second_event_source_id = Guid.NewGuid();
                                    simple_event = new SimpleEvent();
                                    another_simple_event = new AnotherSimpleEvent();

                                    event_source = new StatefulEventSource(event_source_id);
                                    second_event_source = new AnotherStatefulEventSource(second_event_source_id);
                                };
    }
}