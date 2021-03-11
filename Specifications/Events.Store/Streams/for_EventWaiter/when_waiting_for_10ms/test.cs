using System;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_EventWaiter.when_waiting_for_10ms
{
    public class test : given.an_event_waiter
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
            token_source.CancelAfter(TimeSpan.FromSeconds(5));
            try
            {
                dead_lock().Wait();
            }
            catch (Exception)
            {
            }
        };

        It should_have_cancelled_dead_lock = () => token_source.IsCancellationRequested.ShouldBeTrue();

        Cleanup clean = () =>
        {
            token_source.Cancel();
            token_source.Dispose();
        };

        static async Task dead_lock()
        {
            Console.WriteLine("Waiting for 0");
            var first_wait = event_waiter.Wait(0, token).ConfigureAwait(false);
            _ = Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                await notify_async(0).ConfigureAwait(false);
            });
            await first_wait;
            Console.WriteLine("Waiting for 1");
            var second_wait = event_waiter.Wait(1, token);
            _ = Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                await notify_async(1).ConfigureAwait(false);
            });
            second_wait.Wait();        
        }
        
        static Task notify_async(StreamPosition pos)
        {
            var tsc = new TaskCompletionSource<bool>();
            Task.Run(() => 
            {
                Console.WriteLine($"Notifying for {pos.Value}");
                event_waiter.Notify(pos);
                Console.WriteLine($"Setting tsc for {pos.Value}");
                tsc.SetResult(true);
            });
            return tsc.Task;
        }
    }
}
