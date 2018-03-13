using Dolittle.Events;

namespace Dolittle.Runtime.Commands.Coordination.Specs
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}