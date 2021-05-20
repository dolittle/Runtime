// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_the_same_stream_but_different_positions
{
    public class and_an_event_at_the_first_position_is_notified : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition first_stream_position;
        static StreamPosition second_stream_position;
        static StreamPosition third_stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("30895544-e4db-4275-93c9-bd54bf3a1a2b");
            stream_id = Guid.Parse("faee5ac7-613e-44bd-b66d-4a140b1f3255");
            first_stream_position = 33;
            second_stream_position = 61;
            third_stream_position = 62;
        };

        static Task first_result;
        static Task second_result;
        static Task third_result;
        Because of = () =>
        {
            first_result = event_watcher.WaitForEvent(scope_id, stream_id, first_stream_position, cancellation_token);
            second_result = event_watcher.WaitForEvent(scope_id, stream_id, second_stream_position, cancellation_token);
            third_result = event_watcher.WaitForEvent(scope_id, stream_id, third_stream_position, cancellation_token);
            event_watcher.NotifyForEvent(scope_id, stream_id, first_stream_position);
            Thread.Sleep(10);
        };

        It first_should_be_completed = () => first_result.IsCompleted.ShouldBeTrue();
        It second_should_not_be_completed = () => second_result.IsCompleted.ShouldBeFalse();
        It third_should_not_be_completed = () => third_result.IsCompleted.ShouldBeFalse();
    }
}
