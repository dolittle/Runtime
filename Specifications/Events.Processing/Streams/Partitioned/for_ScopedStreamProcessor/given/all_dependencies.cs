// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.given
{
    public class all_dependencies
    {
        static ScopedStreamProcessor stream_processor;
        static IStreamEventWatcher event_waiter;
        protected static EventProcessorId event_processor_id;
        protected static ScopeId scope_id;
        protected static TenantId tenant_id;
        protected static StreamId source_stream_id;
        protected static StreamProcessorId stream_processor_id;
        protected static IResilientStreamProcessorStateRepository stream_processor_state_repository;
        protected static Mock<ICanFetchEventsFromPartitionedStream> events_fetcher;
        protected static IFailingPartitions failing_partitiones;
        protected static Mock<IStreamProcessors> stream_processors;
        protected static Mock<IEventProcessor> event_processor;
        protected static CancellationTokenSource cancellation_token_source;

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
            events_fetcher = new Mock<ICanFetchEventsFromPartitionedStream>();
            event_processor = new Mock<IEventProcessor>();
            event_processor.SetupGet(_ => _.Identifier).Returns(event_processor_id);
            event_processor.SetupGet(_ => _.Scope).Returns(scope_id);
            stream_processors = new Mock<IStreamProcessors>();
            failing_partitiones = new FailingPartitions(
                stream_processor_state_repository,
                event_processor.Object,
                events_fetcher.Object,
                events_fetcher_policy);
            
            event_waiter = new StreamEventWatcher(Mock.Of<ILogger>());
            stream_processor = new ScopedStreamProcessor(
                tenant_id,
                stream_processor_id,
                new StreamDefinition(new FilterDefinition(source_stream_id, stream_processor_id.EventProcessorId.Value, true)),
                StreamProcessorState.New,
                event_processor.Object,
                stream_processor_state_repository,
                events_fetcher.Object,
                failing_partitiones,
                events_fetcher_policy,
                event_waiter,
                new TimeToRetryForPartitionedStreamProcessor(),
                Mock.Of<ILogger<ScopedStreamProcessor>>());
        };
        Cleanup clean = () => cancellation_token_source.Dispose();

        protected static Task start_stream_processor_and_cancel_after(TimeSpan cancelAfter)
        {
            cancellation_token_source.CancelAfter(cancelAfter);
            return stream_processor.Start(cancellation_token_source.Token);
        }
        protected static Task start_stream_processor_set_position_after_and_cancel_after(TimeSpan setPositionAfter, StreamPosition position, TimeSpan cancelAfter)
        {
            var result = stream_processor.Start(cancellation_token_source.Token);
            Task.Delay(setPositionAfter).GetAwaiter().GetResult();
            stream_processor.SetToPosition(position);
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
}
