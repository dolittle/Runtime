// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamEventWatcher
{
    public class when_it_timeouts
    {
        static StreamEventWatcher event_watcher;
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;
        static TimeSpan timeout;
        static CancellationTokenSource tokenSource;
        static CancellationToken token;
        static Task result;

        Establish context = () =>
        {
            event_watcher = new StreamEventWatcher(Moq.Mock.Of<ILogger<StreamEventWatcher>>());
            scope_id = Guid.NewGuid();
            stream_id = Guid.NewGuid();
            stream_position = 0;
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            timeout = TimeSpan.FromMilliseconds(500);
        };

        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, timeout, token);
            result.Wait();
        };

        It should_have_timed_out = () => result.IsCompleted.ShouldBeTrue();
        Cleanup clean = () =>
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
        };
    }
}
