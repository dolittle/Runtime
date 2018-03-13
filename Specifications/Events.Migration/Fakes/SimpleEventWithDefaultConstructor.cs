using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration.Specs.Fakes
{
    public class SimpleEventWithDefaultConstructor : IEvent
    {
        public EventSourceId EventSourceId { get; set; }
    }
}