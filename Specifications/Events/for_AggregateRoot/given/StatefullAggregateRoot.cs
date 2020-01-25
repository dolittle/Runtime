// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Dolittle.Events;
using Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot.given
{
    public class StatefullAggregateRoot : AggregateRoot
    {
        public StatefullAggregateRoot(EventSourceId eventSource)
            : base(eventSource)
        {
            SimpleEventOnCalled = 0;
            AnotherEventOnCalled = 0;
        }

        public int SimpleEventOnCalled { get; private set; }

        public int AnotherEventOnCalled { get; private set; }

        public MethodInfo OnSimpleEventMethod => GetMethodInfoFromCustomPointer("e9a77fc2-72ba-40e9-be2c-39b7198d0de7");

        public MethodInfo OnAnotherEventMethod => GetMethodInfoFromCustomPointer("1900ae56-ad66-4a80-936a-016574b298d5");

        [CustomMethodPointer("e9a77fc2-72ba-40e9-be2c-39b7198d0de7")]
        void On(SimpleEvent @event)
        {
            SimpleEventOnCalled++;
        }

        [CustomMethodPointer("1900ae56-ad66-4a80-936a-016574b298d5")]
        void On(AnotherEvent @event)
        {
            AnotherEventOnCalled++;
        }

        MethodInfo GetMethodInfoFromCustomPointer(string id)
        {
            foreach (var method in typeof(StatefullAggregateRoot).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var pointer = method.GetCustomAttribute<CustomMethodPointerAttribute>();
                if (pointer != null && pointer.Id == id)
                {
                    return method;
                }
            }

            return null;
        }
    }
}