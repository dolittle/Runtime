// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.to_be_appended;

public class and_an_event_in_an_other_stream_is_notified : given.all_dependencies
{
    static ScopeId scope_id;
    static StreamId stream_id;
    static StreamId an_other_stream_id;
    static StreamPosition stream_position;

    Establish context = () =>
    {
        scope_id = Guid.Parse("5ec4e2e3-5bc0-44b9-ad92-23656d38de5a");
        stream_id = Guid.Parse("cd88d089-ad0b-4de1-a3cb-b5944cab8d5e");
        an_other_stream_id = Guid.Parse("7d1e2b1a-06a5-4615-a4c0-d6ff3ed869c4");
        stream_position = 87;
    };

    static Task result;
    Because of = () =>
    {
        result = event_watcher.WaitForEvent(scope_id, stream_id, cancellation_token);
        event_watcher.NotifyForEvent(scope_id, an_other_stream_id, stream_position);
        Thread.Sleep(100);
    };

    It should_not_be_completed = () => result.IsCompleted.ShouldBeFalse();
}