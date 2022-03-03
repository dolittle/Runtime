// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing.for_EventProcessor.given;

public class all_dependencies
{
    protected static ConsentId consent_id;
    protected static SubscriptionId subscription_id;
    protected static IAsyncPolicyFor<EventProcessor> event_processor_policy;
    protected static Mock<IWriteEventHorizonEvents> event_horizon_events_writer;
    protected static IMetricsCollector metrics;
    protected static ILogger logger;
    protected static CommittedEvent @event;
    protected static PartitionId partition;

    Establish context = () =>
    {
        event_processor_policy = new AsyncPolicyFor<EventProcessor>(new EventProcessorPolicy(Mock.Of<ILogger<EventProcessor>>()).Define());

        consent_id = Guid.NewGuid();

        subscription_id = new SubscriptionId(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "partition id");

        partition = "another partition id";

        event_horizon_events_writer = new Mock<IWriteEventHorizonEvents>();

        metrics = Mock.Of<IMetricsCollector>();
        logger = NullLogger.Instance;

        @event = new CommittedEvent(
            0,
            DateTimeOffset.Now,
            "event source id",
            execution_contexts.create(),
            new Artifacts.Artifact(Guid.NewGuid(), 0),
            true,
            "");
    };
}