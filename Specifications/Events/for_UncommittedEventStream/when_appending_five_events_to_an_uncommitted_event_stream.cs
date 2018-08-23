using System.Collections.Generic;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Specs.for_UncommittedEventStream
{
    public class when_appending_five_events_to_an_uncommitted_event_stream : given.an_empty_uncommitted_event_stream
    {
        static List<VersionedEvent> events_and_envelopes;

        Establish context =
            () =>
                {
                    var version = EventSourceVersion.Initial;
                    events_and_envelopes = new List<VersionedEvent>();
                    for (var i = 0; i < 5; i++ )
                    {
                        var @event = new SimpleEvent();
                        events_and_envelopes.Add(new VersionedEvent(@event, version));
                        version = version.NextSequence();
                    }
                };

        Because of = () => events_and_envelopes.ForEach(e => event_stream.Append(e.Event, e.Version));

        It should_have_events = () => event_stream.HasEvents.ShouldBeTrue();
        It should_have_an_event_count_of_5 = () => event_stream.Count.ShouldEqual(5);
    }
}