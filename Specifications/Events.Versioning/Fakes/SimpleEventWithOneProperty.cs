using System;
using doLittle.Events;

namespace doLittle.Runtime.Events.Versioning.Specs.Fakes
{
    public class SimpleEventWithOneProperty : IEvent
    {
        public string SomeString { get; set; }
    }
}