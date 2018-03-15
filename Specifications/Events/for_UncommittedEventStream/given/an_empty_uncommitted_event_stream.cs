using System;
using Dolittle.Events;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Specs.for_UncommittedEventStream.given
{
    public abstract class an_empty_uncommitted_event_stream
    {
        protected static UncommittedEventStream event_stream;
        protected static EventSourceId event_source_id;
        protected static Mock<IEventSource> event_source;

        Establish context = () =>
                {
                    event_source_id = Guid.NewGuid();
                    event_source = new Mock<IEventSource>();
                    event_source.SetupGet(e => e.EventSourceId).Returns(event_source_id);
                    
                    event_stream = new UncommittedEventStream(event_source.Object);
                };
    }
}