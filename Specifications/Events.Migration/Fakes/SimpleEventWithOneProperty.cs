using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration.Specs.Fakes
{
    public class SimpleEventWithOneProperty : IEvent
    {
        public string SomeString { get; set; }
    }
}