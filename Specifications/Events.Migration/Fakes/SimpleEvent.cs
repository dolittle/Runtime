using doLittle.Events;

namespace doLittle.Runtime.Events.Migration.Specs.Fakes
{
    public class SimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}