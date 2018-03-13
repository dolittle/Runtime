using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Specs.for_EventSource
{
    public class SimpleEventWithOneProperty : IEvent
    {
        public string SomeString { get; set; }
    }
}