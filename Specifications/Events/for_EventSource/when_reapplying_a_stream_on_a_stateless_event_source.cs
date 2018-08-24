using System;
using Dolittle.Runtime.Events;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Events.Specs.for_EventSource
{
    public class when_reapplying_a_stream_on_a_stateless_event_source : given.a_stateless_event_source
    {
        static CommittedEventStream event_stream;
        static Exception exception;

        Establish context = () => 
        {
            var events = new [] { build_committed_event(a_versioned_event_source, new SimpleEvent(), new Runtime.Events.Store.CommittedEventVersion(1,1,0)) };
            event_stream = new CommittedEventStream(a_versioned_event_source.EventSource,events);
        };

        Because of = () => exception = Catch.Exception(() => event_source.ReApply(event_stream));

        It should_not_throw_an_exception = () => exception.ShouldBeNull();
    }
}
