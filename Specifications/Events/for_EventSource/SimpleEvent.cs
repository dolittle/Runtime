using Dolittle.Events;

namespace Dolittle.Events.Specs.for_EventSource
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}