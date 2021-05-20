// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.at_a_specific_position
{
    public class and_an_event_at_the_same_position_is_notified : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("bd8bdda5-a5cb-4c35-a2d8-3d96713c43cc");
            stream_id = Guid.Parse("48e0f818-d9d5-4022-aed3-e47a1a506a9d");
            stream_position = 21;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, cancellation_token);
            event_watcher.NotifyForEvent(scope_id, stream_id, stream_position);
            Thread.Sleep(10);
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    }
}
