// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsWriter.when_writing
{
    public class an_event_and_cancellation_is_requested : given.all_dependencies
    {
        static CommittedEvent committed_event;
        static CancellationTokenSource cancellation_token_source;
        static Exception exception;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_event(0);
            cancellation_token_source = new CancellationTokenSource(0);
        };

        Because of = () => exception = Catch.Exception(() => public_events_writer.Write(committed_event, StreamId.PublicEventsId, PartitionId.NotSet, cancellation_token_source.Token).GetAwaiter().GetResult());

        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_the_task_was_canceled = () => exception.ShouldBeOfExactType<TaskCanceledException>();
    }
}