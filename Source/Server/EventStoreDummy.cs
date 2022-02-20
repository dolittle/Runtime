// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;

namespace Server;

[PrivateService]
public class EventStoreDummy : EventStore.EventStoreBase
{
    public EventStoreDummy()
    {
        Console.WriteLine("Constructing the EventStoreDummy Service");
    }

    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request, ServerCallContext context)
    {
        Console.WriteLine("Received commit request on EventStore");
        throw new NotImplementedException();
    }
}
