// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_the_same_stream_and_position;

public class and_an_event_at_the_same_position_is_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("f5bf47e4-7daf-444f-b01c-5b48273b2ecc");
        stream_id = Guid.Parse("778a0d07-b62e-4686-9ee6-a573ae5dd9f3");
        stream_position = 54;
    };

    static Task first_result;
    static Task second_result;
    static Task third_result;
    Because of = () =>
    {
        first_result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, cancellation_token);
        second_result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, cancellation_token);
        third_result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, cancellation_token);
        event_watcher.NotifyForEvent(scope_id, stream_id, stream_position);
        Thread.Sleep(100);
    };

    It first_should_be_completed = () => first_result.IsCompleted.ShouldBeTrue();
    It second_should_be_completed = () => second_result.IsCompleted.ShouldBeTrue();
    It third_should_be_completed = () => third_result.IsCompleted.ShouldBeTrue();
}