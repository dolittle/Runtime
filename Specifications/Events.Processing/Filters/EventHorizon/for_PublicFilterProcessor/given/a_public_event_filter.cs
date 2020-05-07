// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Services;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon.for_PublicFilterProcessor.given
{
    public class a_public_event_filter
    {
        protected static readonly CommittedEvent a_public_event =
            new CommittedEvent(
                EventLogSequenceNumber.Initial,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                execution_contexts.create(),
                new Artifact(Guid.NewGuid(), 0),
                true,
                "");

        protected static readonly CommittedEvent a_non_public_event =
            new CommittedEvent(
                EventLogSequenceNumber.Initial,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                execution_contexts.create(),
                new Artifact(Guid.NewGuid(), 0),
                false,
                "");

        protected static Mock<IReverseCallDispatcher<PublicFiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse>> dispatcher;

        protected static PublicFilterProcessor filter;

        Establish context = () =>
        {
            dispatcher = new Mock<IReverseCallDispatcher<PublicFiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse>>();
            filter = new PublicFilterProcessor(
                new PublicFilterDefinition(Guid.NewGuid(), Guid.NewGuid()),
                dispatcher.Object,
                Mock.Of<IWriteEventsToPublicStreams>(),
                Mock.Of<ILogger>());
        };
    }
}