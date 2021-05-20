// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_public_event
{
    public class and_timeout_is_reached : given.all_dependencies
    {
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            stream_id = Guid.Parse("b396ceca-af95-44c9-8ef4-ebac3e5a677b");
            stream_position = 1;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(stream_id, stream_position, TimeSpan.FromMilliseconds(5), cancellation_token);
            Thread.Sleep(10);
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    }
}
