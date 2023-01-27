// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.to_be_appended;

public class and_a_public_event_in_the_same_stream_is_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("3250b0ca-de3a-49d7-b916-9063a149f4a8");
        stream_id = Guid.Parse("459880c8-4d11-474f-9bbb-2718b36f3651");
        stream_position = 985;
    };

    static Task result;
    Because of = () =>
    {
        result = event_watcher.WaitForEvent(scope_id, stream_id, cancellation_token);
        event_watcher.NotifyForEvent(stream_id, stream_position);
        Thread.Sleep(100);
    };

    It should_not_be_completed = () => result.IsCompleted.Should().BeFalse();
}