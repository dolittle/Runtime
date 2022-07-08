// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_EventWaiter.when_waiting_for_10ms;

public class and_a_later_position_is_later_notified : given.an_event_waiter
{
    static StreamPosition last_position;
    static CancellationTokenSource token_source;
    static CancellationToken token;
    static IList<Task> tasks;

    Establish context = () =>
    {
        token_source = new CancellationTokenSource();
        token = token_source.Token;
        last_position = 3;
        tasks = new List<Task>();
    };

    Because of = () =>
    {
        for (ulong i = 0; i <= last_position.Value; i++)
        {
            tasks.Add(event_waiter.Wait(i, token));
        }

        event_waiter.Notify(last_position.Value + 1);
        Thread.Sleep(100);
    };

    It should_be_done_waiting_for_all_events = () => tasks.ShouldEachConformTo(_ => _.IsCompletedSuccessfully);

    Cleanup clean = () =>
    {
        token_source.Cancel();
        token_source.Dispose();
    };
}