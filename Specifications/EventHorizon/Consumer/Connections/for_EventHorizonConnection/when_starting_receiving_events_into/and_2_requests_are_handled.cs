// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Contracts;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnection.when_starting_receiving_events_into
{
    public class and_2_requests_are_handled : given.all_dependencies
    {
        Establish context = () =>
        {
            var requests = new[]
            {
                CreateRequest(),
                CreateRequest()
            };
            SetupReverseCallClient(requests);
        };
        static Task result;
        Because of = () =>
        {
            result = connection.StartReceivingEventsInto(event_queue, cts.Token);
            Task.Delay(100).GetAwaiter().GetResult();
            cts.Cancel();
            Task.Delay(50).GetAwaiter().GetResult();
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
        It should_not_have_put_anything_in_event_queue = () => event_queue.OutputAvailable().ShouldBeTrue();
        It should_have_2_events_in_queue = () => event_queue.GetConsumingEnumerable().Count().ShouldEqual(2);
    }
}