/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
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
            head = new Head(Guid.NewGuid(),"",42,"",new string[0], DateTimeOffset.UtcNow);
            head.Disconnected += (c) => disconnected = true;

            disconnected_method = typeof(Head).GetMethod("OnDisconnected",BindingFlags.Instance|BindingFlags.NonPublic);
        };

        Because of = () => disconnected_method.Invoke(head, new object[0]);

        It should_trigger_disconnected_event = () => disconnected.ShouldBeTrue();
    }
}