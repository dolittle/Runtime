// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.at_a_specific_position
{
    public class and_nothing_is_notified : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("07189504-dc0e-4819-a438-41e3657e20c5");
            stream_id = Guid.Parse("1f4512d9-e1af-4c03-ad42-90fd03236c9d");
            stream_position = 42;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, cancellation_token);
            Thread.Sleep(10);
        };

        It should_not_be_completed = () => result.IsCompleted.ShouldBeFalse();
    }
}
