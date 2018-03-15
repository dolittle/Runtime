using System;
using Dolittle.Events;

namespace Dolittle.Events.Specs.for_EventSource
{
    public class SimpleEventWithOneProperty : IEvent
    {
        public string SomeString { get; set; }
    }
}