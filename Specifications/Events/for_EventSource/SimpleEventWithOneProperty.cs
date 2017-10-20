using System;
using doLittle.Events;

namespace doLittle.Runtime.Events.Specs.for_EventSource
{
    public class SimpleEventWithOneProperty : IEvent
    {
        public string SomeString { get; set; }
    }
}