// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using Dolittle.Runtime.Events.Processing.Streams;
using System.Threading;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.Streams;
using System.Threading.Channels;
using Microsoft.Extensions.Logging.Abstractions;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing.for_StreamProcessor.given;

public class all_dependencies
{
    protected static SubscriptionId subscription_id;
    protected static CancellationToken cancellation_token;
    protected static Mock<IEventProcessor> event_processor;
    protected static EventsFromEventHorizonFetcher event_fetcher;
    protected static Channel<StreamEvent> event_queue;
    protected static Mock<IStreamProcessorStates> stream_processor_states;
    protected static StreamProcessor stream_processor;
    protected static ExecutionContext execution_context;

    Establish context = () =>
    {
        execution_context = execution_contexts.create();
        subscription_id = new SubscriptionId(
            "aa6403de-6dee-4db8-80a0-366bb9532b3a",
            Guid.Parse("8aef7b88-db5b-4e17-92f7-cfd1cb23a829"),
            "1da8200b-6da6-44fc-a442-29210fb5b934",
            "51355173-6c09-4efa-bd4f-122c6cbc67fc",
            Guid.Parse("cd2433d3-4aad-41ac-87bc-f37240736444"),
            "064749fc-8d13-4b12-a3c4-584e34596b99"
        );
        cancellation_token = CancellationToken.None;
        event_processor = new Mock<IEventProcessor>();
        event_queue = Channel.CreateBounded<StreamEvent>(1000);
        event_fetcher = new EventsFromEventHorizonFetcher(event_queue, Mock.Of<IMetricsCollector>());
        stream_processor_states = new Mock<IStreamProcessorStates>();
        stream_processor = new StreamProcessor(
            subscription_id,
            execution_context,
            event_processor.Object,
            event_fetcher,
            stream_processor_states.Object,
            new EventFetcherPolicies(NullLogger.Instance),
            Mock.Of<IMetricsCollector>(),
            NullLoggerFactory.Instance);
    };
}