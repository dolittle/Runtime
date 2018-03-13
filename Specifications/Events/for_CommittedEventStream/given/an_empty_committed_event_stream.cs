using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedEventStream.given
{
    public abstract class an_empty_committed_event_stream
    {
        protected static CommittedEventStream event_stream;
        protected static EventSourceId event_source_id;

        Establish context = () =>
                {
                    event_source_id = Guid.NewGuid();
                    event_stream = new CommittedEventStream(event_source_id);
                };
    }
}