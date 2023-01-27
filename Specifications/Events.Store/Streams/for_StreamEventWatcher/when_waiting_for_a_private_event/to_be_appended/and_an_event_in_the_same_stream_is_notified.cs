// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.to_be_appended;

public class and_an_event_in_the_same_stream_is_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("bf462ee5-a5a6-4e4f-abff-224e1fcc08ef");
        stream_id = Guid.Parse("ad543636-7f38-442e-a583-ac861f9997ca");
        stream_position = 12;
    };

    static Task result;
    Because of = () =>
    {
        result = event_watcher.WaitForEvent(scope_id, stream_id, cancellation_token);
        event_watcher.NotifyForEvent(scope_id, stream_id, stream_position);
        Thread.Sleep(100);
    };

    It should_be_completed = () => result.IsCompleted.Should().BeTrue();
}