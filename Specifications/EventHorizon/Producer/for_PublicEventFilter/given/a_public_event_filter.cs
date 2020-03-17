// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.EventHorizon.Producer.for_PublicEventFilter.given
{
    public class a_public_event_filter
    {
        protected static readonly CommittedEvent a_public_event =
            new CommittedEvent(
                EventLogSequenceNumber.Initial,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                new Artifact(Guid.NewGuid(), 0),
                true,
                "");

        protected static readonly CommittedEvent a_non_public_event =
            new CommittedEvent(
                EventLogSequenceNumber.Initial,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                new Artifact(Guid.NewGuid(), 0),
                false,
                "");

        protected static PublicEventFilter filter;

        Establish context = () => filter = new PublicEventFilter(Mock.Of<IWriteEventsToStreams>(), Mock.Of<ILogger>());
    }
}