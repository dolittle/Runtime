// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_multiple_waiters_are_waiting.at_the_same_stream_but_different_scopes
{
    public class and_an_event_in_an_other_scope_is_notified : given.all_dependencies
    {
        static ScopeId first_scope_id;
        static ScopeId second_scope_id;
        static ScopeId an_other_scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            first_scope_id = Guid.Parse("f2a0627c-50df-4e1e-87c6-484ef393a8b6");
            second_scope_id = Guid.Parse("24416d3e-b931-4f0b-99ba-b2a0ed0ac617");
            an_other_scope_id = Guid.Parse("d6910d98-960b-4187-9213-bcf57bd03bc0");
            stream_id = Guid.Parse("d1008c81-4140-445d-bf5c-3cc712d4d5ef");
            stream_position = 785;
        };

        static Task first_result;
        static Task second_result;
        static Task third_result;
        Because of = () =>
        {
            first_result = event_watcher.WaitForEvent(first_scope_id, stream_id, stream_position, cancellation_token);
            second_result = event_watcher.WaitForEvent(second_scope_id, stream_id, stream_position, cancellation_token);
            third_result = event_watcher.WaitForEvent(second_scope_id, stream_id, stream_position, cancellation_token);
            event_watcher.NotifyForEvent(an_other_scope_id, stream_id, stream_position);
            Thread.Sleep(10);
        };

        It first_should_not_be_completed = () => first_result.IsCompleted.ShouldBeFalse();
        It second_should_not_be_completed = () => second_result.IsCompleted.ShouldBeFalse();
        It third_should_not_be_completed = () => third_result.IsCompleted.ShouldBeFalse();
    }
}
