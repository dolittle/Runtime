/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Reflection;
using Machine.Specifications;

namespace Dolittle.Runtime.Applications.for_Client
{
    public class when_disconnected
    {
        static Client client;
        static MethodInfo disconnected_method;
        static bool disconnected = false;

        Establish context = () => 
        {
            client = new Client(Guid.NewGuid(),"",42,"",new string[0], DateTimeOffset.UtcNow);
            client.Disconnected += (c) => disconnected = true;

            disconnected_method = typeof(Client).GetMethod("OnDisconnected",BindingFlags.Instance|BindingFlags.NonPublic);
        };

        Because of = () => disconnected_method.Invoke(client, new object[0]);

        It should_trigger_disconnected_event = () => disconnected.ShouldBeTrue();
    }
}