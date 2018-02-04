using doLittle.Events;

namespace doLittle.Runtime.Commands.Coordination.Specs
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}