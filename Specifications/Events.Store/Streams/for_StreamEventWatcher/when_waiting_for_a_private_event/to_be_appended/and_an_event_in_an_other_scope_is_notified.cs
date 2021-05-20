// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.to_be_appended
{
    public class and_an_event_in_an_other_scope_is_notified : given.all_dependencies
    {
        static ScopeId scope_id;
        static ScopeId an_other_scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("401136f4-4aad-465d-8fcb-def02ce4330e");
            an_other_scope_id = Guid.Parse("21e3b367-0551-446e-91fe-ea12f8cb9681");
            stream_id = Guid.Parse("30819979-5a57-4e92-b5c5-b3867d4c36de");
            stream_position = 45632;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, cancellation_token);
            event_watcher.NotifyForEvent(an_other_scope_id, stream_id, stream_position);
            Thread.Sleep(10);
        };

        It should_not_be_completed = () => result.IsCompleted.ShouldBeFalse();
    }
}
