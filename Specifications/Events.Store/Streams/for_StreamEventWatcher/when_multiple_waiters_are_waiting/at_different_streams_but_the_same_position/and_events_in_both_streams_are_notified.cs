// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_different_streams_but_the_same_position;

public class and_events_in_both_streams_are_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId first_stream_id;
    static StreamId second_stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("98937aff-9fee-41d8-99e0-af913dddb26b");
        first_stream_id = Guid.Parse("bc1bac54-47c6-44e5-89a5-e67d1e316a67");
        second_stream_id = Guid.Parse("f3fac4b2-5eeb-4a46-b2cc-33138ade0b9f");
        stream_position = 4651;
    };

    static Task first_result;
    static Task second_result;
    Because of = () =>
    {
        first_result = event_watcher.WaitForEvent(scope_id, first_stream_id, stream_position, cancellation_token);
        second_result = event_watcher.WaitForEvent(scope_id, second_stream_id, stream_position, cancellation_token);
        event_watcher.NotifyForEvent(scope_id, second_stream_id, stream_position);
        event_watcher.NotifyForEvent(scope_id, first_stream_id, stream_position);
        Thread.Sleep(100);
    };

    It first_should_be_completed = () => first_result.IsCompleted.Should().BeTrue();
    It second_should_be_completed = () => second_result.IsCompleted.Should().BeTrue();
}