// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_different_streams_but_the_same_position;

public class and_an_event_in_an_other_stream_is_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId first_stream_id;
    static StreamId second_stream_id;
    static StreamId an_other_stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("48c9e28d-ba37-4ad7-adb3-d66a6f3cd6a4");
        first_stream_id = Guid.Parse("5e02ce85-b906-4651-bb6c-8e2cf1f5a9e2");
        second_stream_id = Guid.Parse("e2639452-0f8f-4337-9572-9c0f73261332");
        an_other_stream_id = Guid.Parse("13a735cd-f0b6-49f3-9ecc-289fb1f5a66f");
        stream_position = 65;
    };

    static Task first_result;
    static Task second_result;
    Because of = () =>
    {
        first_result = event_watcher.WaitForEvent(scope_id, first_stream_id, stream_position, cancellation_token);
        second_result = event_watcher.WaitForEvent(scope_id, second_stream_id, stream_position, cancellation_token);
        event_watcher.NotifyForEvent(scope_id, an_other_stream_id, stream_position);
        Thread.Sleep(100);
    };

    It first_should_not_be_completed = () => first_result.IsCompleted.Should().BeFalse();
    It second_should_not_be_completed = () => second_result.IsCompleted.Should().BeFalse();
}