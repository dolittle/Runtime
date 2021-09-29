// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.at_a_specific_position
{
    public class and_token_is_cancelled : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("88dfa5e0-c605-462f-8ff6-ceca99308b50");
            stream_id = Guid.Parse("68d8bf37-8599-43f1-bf63-e79a4b065f06");
            stream_position = 111;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, stream_position, cancellation_token);
            cancellation_token_source.Cancel();
            Thread.Sleep(100);
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    }
}
