using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration.Specs.Fakes
{
    public class AnotherSimpleEvent : IEvent
    {
        public string Content { get; set; }
    }
}