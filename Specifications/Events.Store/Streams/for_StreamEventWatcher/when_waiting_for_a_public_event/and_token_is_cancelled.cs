// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_public_event;

public class and_token_is_cancelled : given.all_dependencies
{
    static StreamId stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        stream_id = Guid.Parse("f30490e8-cbb0-47b5-81f8-0f0b4efd9ac1");
        stream_position = 131;
    };

    static Task result;
    Because of = () =>
    {
        result = event_watcher.WaitForEvent(stream_id, stream_position, cancellation_token);
        cancellation_token_source.Cancel();
        Thread.Sleep(100);
    };

    It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
}