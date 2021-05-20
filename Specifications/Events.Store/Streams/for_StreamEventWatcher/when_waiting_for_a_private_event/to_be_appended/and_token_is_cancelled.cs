// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.when_waiting_for_a_private_event.to_be_appended
{
    public class and_token_is_cancelled : given.all_dependencies
    {
        static ScopeId scope_id;
        static StreamId stream_id;
        static StreamPosition stream_position;

        Establish context = () =>
        {
            scope_id = Guid.Parse("f619e117-a728-408b-ba09-3f17d53a8de3");
            stream_id = Guid.Parse("105b8a4d-6f8f-45ae-b059-615fd94f27b1");
            stream_position = 3;
        };

        static Task result;
        Because of = () =>
        {
            result = event_watcher.WaitForEvent(scope_id, stream_id, cancellation_token);
            cancellation_token_source.Cancel();
            Thread.Sleep(10);
        };

        It should_be_completed = () => result.IsCompleted.ShouldBeTrue();
    }
}
