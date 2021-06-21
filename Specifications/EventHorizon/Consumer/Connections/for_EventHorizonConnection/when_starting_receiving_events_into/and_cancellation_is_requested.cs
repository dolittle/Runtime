// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Services.Clients;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnection.when_starting_receiving_events_into
{
    public class and_cancellation_is_requested : given.all_dependencies
    {
        Establish context = () =>
        {
            SetupReverseCallClient();
        };
        static Task result;
        Because of = () =>
        {
            result = connection.StartReceivingEventsInto(event_queue, cts.Token);
            Task.Delay(50).GetAwaiter().GetResult();
            cts.Cancel();
            Task.Delay(50).GetAwaiter().GetResult();
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
        It should_not_have_put_anything_in_event_queue = () => event_queue.OutputAvailable().ShouldBeFalse();
    }
}