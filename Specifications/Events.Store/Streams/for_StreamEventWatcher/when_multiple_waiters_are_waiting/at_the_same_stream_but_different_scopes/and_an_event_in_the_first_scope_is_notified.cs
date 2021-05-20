// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_the_same_stream_but_different_scopes
{
    public class and_an_event_in_the_first_scope_is_notified : given.all_dependencies
    {
        static ScopeId first_scope_id;
        static ScopeId second_scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            first_scope_id = Guid.Parse("3a618c09-7cfc-4245-b27f-c1bfabf58621");
            second_scope_id = Guid.Parse("0d33033b-3301-4f64-ae30-8ea251dd1929");
            stream_id = Guid.Parse("a013cdf8-2a8b-4b24-9824-4ac66719805e");
            stream_position = 38;
        };

        static Task first_result;
        static Task second_result;
        static Task third_result;
        Because of = () =>
        {
            first_result = event_watcher.WaitForEvent(first_scope_id, stream_id, stream_position, cancellation_token);
            second_result = event_watcher.WaitForEvent(second_scope_id, stream_id, stream_position, cancellation_token);
            third_result = event_watcher.WaitForEvent(second_scope_id, stream_id, stream_position, cancellation_token);
            event_watcher.NotifyForEvent(first_scope_id, stream_id, stream_position);
            Thread.Sleep(20);
        };

        It first_should_be_completed = () => first_result.IsCompleted.ShouldBeTrue();
        It second_should_not_be_completed = () => second_result.IsCompleted.ShouldBeFalse();
        It third_should_not_be_completed = () => third_result.IsCompleted.ShouldBeFalse();
    }
}
