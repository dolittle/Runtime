// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_EventHorizonEventProcessor.given
{
    public class all_dependencies
    {
        protected static ConsentId consent_id;
        protected static SubscriptionId subscription_id;
        protected static Moq.Mock<IWriteEventHorizonEvents> event_horizon_events_writer;
        protected static CommittedEvent @event;
        protected static PartitionId partition;

        Establish context = () =>
        {
            consent_id = Guid.NewGuid();
            subscription_id = new SubscriptionId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            partition = Guid.NewGuid();
            event_horizon_events_writer = new Moq.Mock<IWriteEventHorizonEvents>();
            @event = new CommittedEvent(
                0,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                execution_contexts.create(),
                new Artifacts.Artifact(Guid.NewGuid(), 0),
                true,
                "");
        };
    }
}