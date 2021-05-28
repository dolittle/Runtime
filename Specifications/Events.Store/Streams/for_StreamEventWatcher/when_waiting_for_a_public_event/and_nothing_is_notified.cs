// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_public_event
{
    public class and_nothing_is_notified : given.all_dependencies
    {
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            stream_id = Guid.Parse("b1be16c8-6453-4f35-bf1b-ee15da09543d");
            stream_position = 27;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(stream_id, stream_position, cancellation_token);
            Thread.Sleep(100);
        };

        It should_not_be_completed = () => result.IsCompleted.ShouldBeFalse();
    }
}
