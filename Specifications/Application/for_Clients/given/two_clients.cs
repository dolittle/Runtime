/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Application.for_Clients.given
{
    public class two_clients
    {
        protected static Client first_client;
        protected static Client second_client;

        Establish context = () =>
        {
            first_client = new Client(Guid.NewGuid(), "first client", 42, "Some runtime", new[] {"first", "second"}, DateTimeOffset.UtcNow);
            second_client = new Client(Guid.NewGuid(), "second client", 43, "Some other runtime", new[] {"third", "fourth"}, DateTimeOffset.UtcNow.AddDays(5));
        };
    }
}