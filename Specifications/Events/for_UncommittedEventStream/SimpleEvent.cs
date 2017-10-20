using doLittle.Events;

namespace doLittle.Runtime.Events.Specs.for_UncommittedEventStream
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}