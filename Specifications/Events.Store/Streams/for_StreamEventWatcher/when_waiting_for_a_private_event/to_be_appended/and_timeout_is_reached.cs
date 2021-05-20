// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.to_be_appended
{
    public class and_timeout_is_reached : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("8a8232f0-0599-4e39-b49c-d16aa2de2d45");
            stream_id = Guid.Parse("2d6190d3-1bab-4b49-92fc-5d2c08ccbfd9");
            stream_position = 3;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, TimeSpan.FromMilliseconds(10), cancellation_token);
            Thread.Sleep(20);
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    }
}
