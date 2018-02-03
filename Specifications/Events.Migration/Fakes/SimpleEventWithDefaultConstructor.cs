using doLittle.Events;

namespace doLittle.Runtime.Events.Migration.Specs.Fakes
{
    public class SimpleEventWithDefaultConstructor : IEvent
    {
        public EventSourceId EventSourceId { get; set; }
    }
}