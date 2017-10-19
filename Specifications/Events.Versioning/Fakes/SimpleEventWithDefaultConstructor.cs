namespace doLittle.Runtime.Events.Versioning.Specs.Fakes
{
    public class SimpleEventWithDefaultConstructor : IEvent
    {
        public EventSourceId EventSourceId { get; set; }
    }
}