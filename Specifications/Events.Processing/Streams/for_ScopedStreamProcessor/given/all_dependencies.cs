// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.Runtime.Events.Processing.Streams.for_ScopedStreamProcessor.given;

public class all_dependencies
{
    static ScopedStreamProcessor stream_processor;
    protected static EventProcessorId event_processor_id;
    protected static ScopeId scope_id;
    protected static TenantId tenant_id;
    protected static StreamId source_stream_id;
    protected static StreamProcessorId stream_processor_id;
    protected static IResilientStreamProcessorStateRepository stream_processor_state_repository;
    protected static Mock<ICanFetchEventsFromStream> events_fetcher;
    protected static Mock<IStreamProcessors> stream_processors;
    protected static Mock<IEventProcessor> event_processor;
    static IStreamEventWatcher event_waiter;
    protected static CancellationTokenSource cancellation_token_source;
    protected static Mock<Func<TenantId, CancellationToken, Task<Try>>> action_to_perform_before_reprocessing;

    Establish context = () =>
    {
        cancellation_token_source = new CancellationTokenSource();
        var events_fetcher_policy = new AsyncPolicyFor<ICanFetchEventsFromStream>(new EventFetcherPolicy(Mock.Of<ILogger<ICanFetchEventsFromStream>>()).Define());
        var in_memory_stream_processor_state_repository = new in_memory_stream_processor_state_repository();
        event_processor_id = Guid.NewGuid();
        scope_id = Guid.NewGuid();
        tenant_id = Guid.NewGuid();
        source_stream_id = Guid.NewGuid();
        stream_processor_id = new StreamProcessorId(scope_id, event_processor_id, source_stream_id);
        stream_processor_state_repository = in_memory_stream_processor_state_repository;
        events_fetcher = new Mock<ICanFetchEventsFromStream>(MockBehavior.Strict);
        event_processor = new Mock<IEventProcessor>(MockBehavior.Strict);
        event_processor.SetupGet(_ => _.Identifier).Returns(event_processor_id);
        event_processor.SetupGet(_ => _.Scope).Returns(scope_id);
        stream_processors = new Mock<IStreamProcessors>(MockBehavior.Strict);

        event_waiter = new StreamEventWatcher(Mock.Of<ILogger>());
        stream_processor = new ScopedStreamProcessor(
            tenant_id,
            stream_processor_id,
            new StreamDefinition(new FilterDefinition(source_stream_id, stream_processor_id.EventProcessorId.Value, false)),
            StreamProcessorState.New,
            event_processor.Object,
            stream_processor_state_repository,
            events_fetcher.Object,
            events_fetcher_policy,
            event_waiter,
            new TimeToRetryForUnpartitionedStreamProcessor(),
            Mock.Of<ILogger<ScopedStreamProcessor>>());

        action_to_perform_before_reprocessing = new Mock<Func<TenantId, CancellationToken, Task<Try>>>();
        action_to_perform_before_reprocessing
            .Setup(_ => _(It.IsAny<TenantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Try.Succeeded());
    };
        
    Cleanup clean = () => cancellation_token_source.Dispose();

    protected static Task start_stream_processor_and_cancel_after(TimeSpan cancelAfter)
    {
        cancellation_token_source.CancelAfter(cancelAfter);
        return stream_processor.Start(cancellation_token_source.Token);
    }
    protected static Task start_stream_processor_set_position_after_and_cancel_after(TimeSpan setPositionAfter, StreamPosition position, Func<TenantId, CancellationToken, Task<Try>> beforeReprocessingAction, TimeSpan cancelAfter)
    {
        var result = stream_processor.Start(cancellation_token_source.Token);
        Task.Delay(setPositionAfter).GetAwaiter().GetResult();
        stream_processor.PerformActionAndReprocessEventsFrom(position, beforeReprocessingAction).GetAwaiter().GetResult();
        cancellation_token_source.CancelAfter(cancelAfter);
        return result;
    }
    protected static StreamProcessorState current_stream_processor_state
        => stream_processor_state_repository.TryGetFor(
                stream_processor_id,
                CancellationToken.None)
            .GetAwaiter()
            .GetResult()
            .Result as StreamProcessorState;

    protected static void setup_event_stream(params Try<StreamEvent>[] streamEvents)
    {
        for (var i = 0; i <= streamEvents.Length; i++)
        {
            var position = new StreamPosition((ulong)i);
            if (i == streamEvents.Length)
            {
                events_fetcher
                    .Setup(_ => _.Fetch(position, Moq.It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(Try<StreamEvent>.Failed(new Exception())));
                break;
            }
            var streamEvent = streamEvents[i];
            events_fetcher
                .Setup(_ => _.Fetch(position, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(streamEvent));
            event_waiter.NotifyForEvent(source_stream_id, position);
        }
    }
}