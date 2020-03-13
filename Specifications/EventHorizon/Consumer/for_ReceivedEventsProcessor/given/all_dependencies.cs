// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_ReceivedEventsProcessor.given
{
    public class all_dependencies
    {
        protected static Microservice microservice;
        protected static TenantId tenant;
        protected static Moq.Mock<IWriteReceivedEvents> received_events_writer;
        protected static CommittedEvent @event;
        protected static PartitionId partition;

        Establish context = () =>
        {
            microservice = Guid.NewGuid();
            tenant = Guid.NewGuid();
            partition = Guid.NewGuid();
            received_events_writer = new Moq.Mock<IWriteReceivedEvents>();
            @event = new CommittedEvent(
                0,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                new Artifacts.Artifact(Guid.NewGuid(), 0),
                true,
                "");
        };
    }
}