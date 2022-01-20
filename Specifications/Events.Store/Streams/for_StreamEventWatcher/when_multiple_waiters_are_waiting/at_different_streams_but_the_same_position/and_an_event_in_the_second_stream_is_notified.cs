// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_different_streams_but_the_same_position;

public class and_an_event_in_the_second_stream_is_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId first_stream_id;
    static StreamId second_stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("a0c68089-6ea0-44d7-a7b4-359b8600e860");
        first_stream_id = Guid.Parse("fa2416cb-6945-460a-a511-239d040824d2");
        second_stream_id = Guid.Parse("5883bcf7-31f7-4a27-b0d8-79463647aa2c");
        stream_position = 987;
    };

    static Task first_result;
    static Task second_result;
    Because of = () =>
    {
        first_result = event_watcher.WaitForEvent(scope_id, first_stream_id, stream_position, cancellation_token);
        second_result = event_watcher.WaitForEvent(scope_id, second_stream_id, stream_position, cancellation_token);
        event_watcher.NotifyForEvent(scope_id, second_stream_id, stream_position);
        Thread.Sleep(100);
    };

    It first_should_not_be_completed = () => first_result.IsCompleted.ShouldBeFalse();
    It second_should_be_completed = () => second_result.IsCompleted.ShouldBeTrue();
}