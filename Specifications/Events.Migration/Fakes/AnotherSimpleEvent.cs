using doLittle.Events;

namespace doLittle.Runtime.Events.Migration.Specs.Fakes
{
    public class AnotherSimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}