using Machine.Specifications;

namespace doLittle.Runtime.Events.Specs.for_EventSource.given
{
	public class an_event_source_with_2_uncommitted_events : a_stateful_event_source
	{
		private Establish context = () =>
			    {
					var firstEvent = new SimpleEvent();
                    event_source.Apply(firstEvent);
					var secondEvent = new SimpleEvent();
                    event_source.Apply(secondEvent);
				};
	}
}