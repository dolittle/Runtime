// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.to_be_appended
{
    public class and_nothing_is_notified : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("bdbb14ed-eefe-4b29-aa1f-7b0e843ab02e");
            stream_id = Guid.Parse("bdad1a39-b873-4501-ba94-85d055436f1b");
            stream_position = 43;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, cancellation_token);
            Thread.Sleep(20);
        };

        It should_not_be_completed = () => result.IsCompleted.ShouldBeFalse();
    }
}
