// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using contracts::Dolittle.Runtime.EventHorizon;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Extension methods for <see cref="EventHorizonEvent" />.
    /// </summary>
    public static class EventHorizonEventExtensions
    {
        /// <summary>
        /// Converts the <see cref="EventHorizonEvent" /> to <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="EventHorizonEvent" />.</param>
        /// <returns>The <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this EventHorizonEvent @event) =>
            new CommittedEvent(
                @event.EventLogSequenceNumber,
                @event.Occurred.ToDateTimeOffset(),
                @event.EventSource.To<EventSourceId>(),
                @event.Correlation.To<CorrelationId>(),
                @event.ProducerMicroservice.To<Microservice>(),
                @event.ProducerTenant.To<TenantId>(),
                new Cause(CauseType.Event, 0),
                new Artifact(@event.Type.Id.To<ArtifactId>(), @event.Type.Generation),
                true,
                @event.Content);
    }
}