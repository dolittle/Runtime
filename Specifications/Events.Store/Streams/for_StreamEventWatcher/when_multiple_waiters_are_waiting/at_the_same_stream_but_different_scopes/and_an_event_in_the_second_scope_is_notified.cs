// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_the_same_stream_but_different_scopes
{
    public class and_an_event_in_the_second_scope_is_notified : given.all_dependencies
    {
        static ScopeId first_scope_id;
        static ScopeId second_scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            first_scope_id = Guid.Parse("546be0e6-b4f3-465d-8735-141cab05d56c");
            second_scope_id = Guid.Parse("e2df49fe-85e2-4028-bf53-bc78b607a99d");
            stream_id = Guid.Parse("330548fd-2985-4dc6-9de1-2c488eb16344");
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
            event_watcher.NotifyForEvent(second_scope_id, stream_id, stream_position);
            Thread.Sleep(20);
        };

        It first_should_not_be_completed = () => first_result.IsCompleted.ShouldBeFalse();
        It second_should_be_completed = () => second_result.IsCompleted.ShouldBeTrue();
        It third_should_be_completed = () => third_result.IsCompleted.ShouldBeTrue();
    }
}
