// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.given
{
    public class all_dependencies
    {
        protected static StreamId source_stream;
        protected static StreamId target_stream;
        protected static EventProcessorId event_processor_id;
        protected static ScopeId scope_id;
        protected static StreamProcessorId stream_processor_id;
        protected static Mock<IFilterProcessor<FilterDefinition>> filter_processor;
        protected static Mock<IEventFetchers> events_fetchers;
        protected static FilterDefinition filter_definition;
        protected static ValidateFilterByComparingStreams validator;
        protected static Mock<ICanFetchRangeOfEventsFromStream> events_from_event_log_fetcher;
        protected static Mock<ICanFetchRangeOfEventsFromStream> events_from_filtered_stream_fetcher;
        protected static IList<StreamEvent> events_in_event_log;
        protected static IList<StreamEvent> events_in_filtered_stream;
        protected static CancellationToken cancellation_token;

        Establish context = () =>
        {
            var mocks = new MockRepository(MockBehavior.Strict);

            source_stream = Guid.Parse("ad154c87-e62d-4bca-a3b7-dab069220447");
            target_stream = Guid.Parse("e8534fc4-a0be-4782-bdba-df6c22a26901");
            event_processor_id = target_stream.Value;
            scope_id = Guid.Parse("411fd266-cfc7-4871-a90d-699cff2c5d2f");
            stream_processor_id = new StreamProcessorId(scope_id, event_processor_id, source_stream);
            filter_processor = mocks.Create<IFilterProcessor<FilterDefinition>>();
            filter_definition = new FilterDefinition(source_stream, target_stream, true);
            filter_processor.SetupGet(_ => _.Definition).Returns(filter_definition);
            filter_processor.SetupGet(_ => _.Identifier).Returns(event_processor_id);
            filter_processor.SetupGet(_ => _.Scope).Returns(scope_id);
            events_fetchers = mocks.Create<IEventFetchers>();
            validator = new ValidateFilterByComparingStreams(events_fetchers.Object);
            events_from_event_log_fetcher = mocks.Create<ICanFetchRangeOfEventsFromStream>();
            events_from_filtered_stream_fetcher = mocks.Create<ICanFetchRangeOfEventsFromStream>();
            events_in_event_log = new List<StreamEvent>();
            events_in_filtered_stream = new List<StreamEvent>();

            events_from_event_log_fetcher
                .Setup(_ => _.FetchRange(Moq.It.IsAny<StreamPositionRange>(), cancellation_token))
                .Returns<StreamPositionRange, CancellationToken>((range, _) => Task.FromResult(events_in_event_log.Where(_ => _.Position >= range.From && _.Position < range.From + range.Length)));
            events_from_filtered_stream_fetcher
                .Setup(_ => _.FetchRange(Moq.It.IsAny<StreamPositionRange>(), cancellation_token))
                .Returns<StreamPositionRange, CancellationToken>((range, _) => Task.FromResult(events_in_filtered_stream.Where(_ => _.Position >= range.From && _.Position < range.From + range.Length)));

            events_fetchers
                .Setup(_ => _.GetRangeFetcherFor(scope_id, new EventLogStreamDefinition(), cancellation_token))
                .Returns(Task.FromResult(events_from_event_log_fetcher.Object));
            events_fetchers
                .Setup(_ => _.GetRangeFetcherFor(scope_id, new StreamDefinition(filter_definition), cancellation_token))
                .Returns(Task.FromResult(events_from_filtered_stream_fetcher.Object));

            cancellation_token = CancellationToken.None;
        };

        protected static void add_event_to_event_log(uint num_events_to_create)
        {
            var end = events_in_event_log.Count + num_events_to_create;
            for (int i = events_in_event_log.Count; i < end; i++)
            {
                events_in_event_log.Add(new StreamEvent(committed_events.single((uint)i), (uint)i, StreamId.EventLog, Guid.Empty, true));
            }
        }

        protected static void add_event_to_filtered_stream(uint num_events_to_create, PartitionId partition = default)
        {
            var end = events_in_filtered_stream.Count + num_events_to_create;
            for (int i = events_in_filtered_stream.Count; i < end; i++)
            {
                events_in_filtered_stream.Add(new StreamEvent(committed_events.single((uint)i), (uint)i, target_stream, partition == default ? new PartitionId(Guid.Empty) : partition, true));
            }
        }

        protected static void add_event_to_event_log(CommittedEvent @event)
        {
            var end = events_in_event_log.Count + 1;
            for (int i = events_in_event_log.Count; i < end; i++)
            {
                events_in_event_log.Add(new StreamEvent(@event, (uint)i, StreamId.EventLog, Guid.Empty, true));
            }
        }

        protected static void add_event_to_filtered_stream(CommittedEvent @event, PartitionId partition = default)
        {
            var end = events_in_filtered_stream.Count + 1;
            for (int i = events_in_filtered_stream.Count; i < end; i++)
            {
                events_in_filtered_stream.Add(new StreamEvent(@event, (uint)i, target_stream, partition == default ? new PartitionId(Guid.Empty) : partition, true));
            }
        }
    }
}
