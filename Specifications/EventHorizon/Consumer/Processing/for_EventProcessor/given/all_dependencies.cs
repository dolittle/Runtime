// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing.for_EventProcessor.given;

public class all_dependencies
{
    protected static ConsentId consent_id;
    protected static SubscriptionId subscription_id;
    protected static Mock<ICommitExternalEvents> external_events_committer;
    protected static IMetricsCollector metrics;
    protected static ILogger logger;
    protected static CommittedEvent @event;
    protected static PartitionId partition;
    protected static ExecutionContext execution_context;

    Establish context = () =>
    {
        execution_context = execution_contexts.create();
        external_events_committer = new Mock<ICommitExternalEvents>();
        consent_id = Guid.NewGuid();
        subscription_id = new SubscriptionId(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "partition id");
        partition = "another partition id";
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