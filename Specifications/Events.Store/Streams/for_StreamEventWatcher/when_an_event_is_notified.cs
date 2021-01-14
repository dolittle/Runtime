// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher
{
    public class when_an_event_is_notified
    {
        static StreamEventWatcher event_watcher;
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;
        static CancellationTokenSource tokenSource;
        static Task result;

        Establish context = () =>
        {
            event_watcher = new StreamEventWatcher();
            scope_id = Guid.NewGuid();
            stream_id = Guid.NewGuid();
            stream_position = 0;
            tokenSource = new CancellationTokenSource();
        };

        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, tokenSource.Token);
            event_watcher.NotifyForEvent(scope_id, stream_id, stream_position);
            result.Wait();
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
        Cleanup clean = () =>
        {
            tokenSource.Dispose();
        };
    }
}
