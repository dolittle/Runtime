// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher
{
    public class when_multiple_waiters_are_notified
    {
        static StreamEventWatcher event_watcher1;
        static StreamEventWatcher event_watcher2;
        static StreamEventWatcher event_watcher3;
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;
        static CancellationTokenSource tokenSource;
        static Task result1;
        static Task result2;
        static Task result3;

        Establish context = () =>
        {
            event_watcher1 = new StreamEventWatcher();
            event_watcher2 = new StreamEventWatcher();
            event_watcher3 = new StreamEventWatcher();
            scope_id = Guid.NewGuid();
            stream_id = Guid.NewGuid();
            stream_position = 0;
            tokenSource = new CancellationTokenSource();
        };

        Because of = () =>
        {
            result1 = event_watcher1.WaitForEvent(scope_id, stream_id, stream_position, tokenSource.Token);
            result2 = event_watcher2.WaitForEvent(scope_id, stream_id, stream_position, tokenSource.Token);
            result3 = event_watcher3.WaitForEvent(scope_id, stream_id, stream_position, tokenSource.Token);

            event_watcher.NotifyForEvent(scope_id, stream_id, stream_position);
            Task.WaitAll(result1, result2, result3);
        };
        It first_should_be_completed = () => result1.IsCompleted.ShouldBeTrue();
        It second_should_be_completed = () => result2.IsCompleted.ShouldBeTrue();
        It thid_should_be_completed = () => result3.IsCompleted.ShouldBeTrue();
    }
}
