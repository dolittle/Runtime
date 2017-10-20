using doLittle.Events;

namespace doLittle.Runtime.Events.Specs.for_CommittedEventStream
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}