// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.at_a_specific_position;

public class and_an_event_at_a_previous_position_is_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("7a3d08bd-d5af-4400-8e2e-8ff37a1d5658");
        stream_id = Guid.Parse("164c589d-8e94-4cc3-bad5-3144b4f2e8a0");
        stream_position = 37;
    };

    static Task result;
    Because of = () =>
    {
        result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, cancellation_token);
        event_watcher.NotifyForEvent(scope_id, stream_id, stream_position - 1);
        Thread.Sleep(100);
    };

    It should_not_be_completed = () => result.IsCompleted.ShouldBeFalse();
}