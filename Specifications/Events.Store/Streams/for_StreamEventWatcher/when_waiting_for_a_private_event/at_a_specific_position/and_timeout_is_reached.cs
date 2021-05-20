// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.at_a_specific_position
{
    public class and_timeout_is_reached : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("9b497c70-3793-4a17-89a4-0a0d107b581b");
            stream_id = Guid.Parse("8a150538-3bca-43f9-af56-a6415d17f034");
            stream_position = 15;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, TimeSpan.FromMilliseconds(5), cancellation_token);
            Thread.Sleep(10);
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    }
}
