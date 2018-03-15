using System;

namespace Dolittle.Events.Specs.for_EventSource
{
    public class StatelessEventSource : EventSource
    {
        public StatelessEventSource(Guid id) : base(id) {}
    }
}