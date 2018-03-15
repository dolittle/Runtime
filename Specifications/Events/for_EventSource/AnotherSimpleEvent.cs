using Dolittle.Events;

namespace Dolittle.Events.Specs.for_EventSource
{
    public class AnotherSimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}