using Dolittle.Events;

namespace Dolittle.Runtime.Events.Specs.for_EventSource
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}