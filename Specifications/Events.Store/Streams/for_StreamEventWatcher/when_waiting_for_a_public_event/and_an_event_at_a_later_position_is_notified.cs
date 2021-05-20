// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_public_event
{
    public class and_an_event_at_a_later_position_is_notified : given.all_dependencies
    {
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            stream_id = Guid.Parse("1748ce3c-1fd2-4a34-8237-fb2201ecb8bb");
            stream_position = 0;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(stream_id, stream_position, cancellation_token);
            event_watcher.NotifyForEvent(stream_id, stream_position + 9);
            Thread.Sleep(20);
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    }
}
