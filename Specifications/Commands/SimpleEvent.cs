using doLittle.Events;

namespace doLittle.Runtime.Commands.Specs
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}