// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Specs.for_EventSource
{
    public class AnotherStatefulEventSource : EventSource
    {
        public string Value { get; set; }

        public string OneProperty { get; set; }

        public bool EventApplied { get; private set; }

        public bool EventWithOnePropertyApplied { get; private set; }

        public AnotherStatefulEventSource(Guid id)
            : base(id)
        {
        }

        void On(AnotherSimpleEvent simpleEvent)
        {
            EventApplied = true;
            Value = simpleEvent.Content;
        }

        void On(SimpleEventWithOneProperty simpleEventWithOneProperty)
        {
            EventWithOnePropertyApplied = true;
            OneProperty = simpleEventWithOneProperty.SomeString;
        }
    }
}