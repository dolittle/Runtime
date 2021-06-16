using System;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_EventWaiter
{
    public class when_trying_to_provoke_dead_lock : given.an_event_waiter
    {
        static CancellationTokenSource token_source;
        static CancellationToken token;

        Establish context = () =>
        {
            token_source = new CancellationTokenSource();
            token = token_source.Token;
        };

        Because of = () =>
        {
            // while (!System.Diagnostics.Debugger.IsAttached) System.Threading.Thread.Sleep(50);
            token_source.CancelAfter(TimeSpan.FromSeconds(3));
            try
            {
                dead_lock().Wait();
            }
            catch (Exception)
            {
            }

        };

        It should_not_be_a_dead_lock = () => token_source.IsCancellationRequested.ShouldBeFalse();

        Cleanup clean = () =>
        {
            token_source.Cancel();
            token_source.Dispose();
        };

        static async Task dead_lock()
        {
            var first_wait = event_waiter.Wait(0, token).ConfigureAwait(false);
            _ = Task.Run(() =>
            {
                event_waiter.Notify(0);
            });
            await first_wait;
            var second_wait = event_waiter.Wait(1, token);
            _ = Task.Run(() =>
            {
                event_waiter.Notify(1);
            });
            second_wait.Wait();
        }
    }
}
