// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Machine.Specifications;

namespace Dolittle.Runtime.Heads.for_Head
{
    public class when_disconnected
    {
        static Head head;
        static MethodInfo disconnected_method;
        static bool disconnected = false;

        Establish context = () =>
        {
            head = new Head(Guid.NewGuid(), "", DateTimeOffset.UtcNow);
            head.Disconnected += (c) => disconnected = true;

            disconnected_method = typeof(Head).GetMethod("OnDisconnected", BindingFlags.Instance | BindingFlags.NonPublic);
        };

        Because of = () => disconnected_method.Invoke(head, Array.Empty<object>());

        It should_trigger_disconnected_event = () => disconnected.ShouldBeTrue();
    }
}