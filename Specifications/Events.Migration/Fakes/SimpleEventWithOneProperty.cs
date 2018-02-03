using System;
using doLittle.Events;

namespace doLittle.Runtime.Events.Migration.Specs.Fakes
{
    public class SimpleEventWithOneProperty : IEvent
    {
        public string SomeString { get; set; }
    }
}