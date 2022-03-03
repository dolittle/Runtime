// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.given;

public class all_dependencies
{
    protected static StreamId stream_id;
    protected static IAsyncPolicyFor<ICanFetchEventsFromStream> events_fetcher_policy;
    protected static Mock<IResilientStreamProcessorStateRepository> stream_processor_state_repository;
    protected static Mock<IEventProcessor> event_processor;
    protected static Mock<ICanFetchEventsFromPartitionedStream> events_fetcher;

    Establish context = () =>
    {
        events_fetcher_policy = new AsyncPolicyFor<ICanFetchEventsFromStream>(new EventFetcherPolicy(Mock.Of<ILogger<ICanFetchEventsFromStream>>()).Define());
        stream_id = Guid.NewGuid();
        event_processor = new Mock<IEventProcessor>();
        stream_processor_state_repository = new Mock<IResilientStreamProcessorStateRepository>();
        events_fetcher = new Mock<ICanFetchEventsFromPartitionedStream>();
    };
}