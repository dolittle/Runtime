// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_different_streams_but_the_same_position;

public class and_an_event_in_the_first_stream_is_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId first_stream_id;
    static StreamId second_stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("5458c4c8-4c90-4413-a212-be02fdb6209e");
        first_stream_id = Guid.Parse("8b6b2495-3c40-49d1-a5ea-c8d2c69cb218");
        second_stream_id = Guid.Parse("72bf035d-8641-40ad-8af1-a93d23f3903c");
        stream_position = 54;
    };

    static Task first_result;
    static Task second_result;
    Because of = () =>
    {
        first_result = event_watcher.WaitForEvent(scope_id, first_stream_id, stream_position, cancellation_token);
        second_result = event_watcher.WaitForEvent(scope_id, second_stream_id, stream_position, cancellation_token);
        event_watcher.NotifyForEvent(scope_id, first_stream_id, stream_position);
        Thread.Sleep(100);
    };

    It first_should_be_completed = () => first_result.IsCompleted.Should().BeTrue();
    It second_should_not_be_completed = () => second_result.IsCompleted.Should().BeFalse();
}