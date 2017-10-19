using System;

namespace doLittle.Runtime.Events.Specs.for_EventSource
{
    public class StatelessEventSource : EventSource
    {
        public StatelessEventSource(Guid id) : base(id) {}
    }
}