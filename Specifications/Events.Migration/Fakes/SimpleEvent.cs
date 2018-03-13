using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration.Specs.Fakes
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}