// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_EventWaiter.when_waiting
{
    public class and_it_is_never_notified : given.an_event_waiter
    {
        static StreamPosition position;
        static CancellationToken token;
        static Task result;

        Establish context = () =>
        {
            position = 2;
            token = default;
        };

        Because of = () => result = event_waiter.Wait(position, token);

        It should_wait_for_event = () => result.IsCompleted.ShouldBeFalse();

        Cleanup clean = () =>
        {
            token.
        };
    }
}