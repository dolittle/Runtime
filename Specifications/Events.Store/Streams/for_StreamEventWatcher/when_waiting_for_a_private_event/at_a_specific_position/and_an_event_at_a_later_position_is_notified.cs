// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.at_a_specific_position
{
    public class and_an_event_at_a_later_position_is_notified : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("6e4ffb69-bd00-4aa5-883b-adb0388c1078");
            stream_id = Guid.Parse("b80c70bc-45c4-4637-86a5-000aaf109b91");
            stream_position = 23;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, cancellation_token);
            event_watcher.NotifyForEvent(scope_id, stream_id, stream_position + 13);
            Thread.Sleep(20);
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    }
}
