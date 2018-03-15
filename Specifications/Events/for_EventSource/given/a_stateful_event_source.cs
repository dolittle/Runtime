using Dolittle.Runtime.Events;
using Machine.Specifications;
using Moq;
using System;

namespace Dolittle.Events.Specs.for_EventSource.given
{
    public class a_stateful_event_source
	{
		protected static StatefulEventSource event_source;
		protected static EventSourceId event_source_id;
		protected static IEvent @event;
        protected static Mock<IEventEnvelope> event_envelope;

		Establish context = () =>
				{
					event_source_id = Guid.NewGuid();
					event_source = new StatefulEventSource(event_source_id);
                    event_envelope = new Mock<IEventEnvelope>();
				};
	}
}