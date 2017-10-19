using System;

namespace doLittle.Runtime.Events.Specs.for_EventSource
{
    public class SimpleEventWithOneProperty : IEvent
    {
        public string SomeString { get; set; }
    }
}