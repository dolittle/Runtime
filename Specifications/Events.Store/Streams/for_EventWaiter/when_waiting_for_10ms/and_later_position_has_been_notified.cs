// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_EventWaiter.when_waiting_for_10ms
{
    public class and_later_position_has_been_notified : given.an_event_waiter
    {
        static StreamPosition position;
        static CancellationTokenSource token_source;
        static CancellationToken token;
        static Task result;

        Establish context = () =>
        {
            position = 2;
            token_source = new CancellationTokenSource();
            token = token_source.Token;
            event_waiter.Notify(5);
        };

        Because of = () =>
        {
            result = event_waiter.Wait(position, token);
            Thread.Sleep(100);
        };

        It should_be_done_waiting_for_event = () => result.IsCompleted.ShouldBeTrue();

        Cleanup clean = () =>
        {
            token_source.Cancel();
            token_source.Dispose();
        };
    }
}
