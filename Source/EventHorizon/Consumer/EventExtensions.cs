// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.ApplicationModel;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Security;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Extension methods for <see cref="Contracts.EventHorizonEvent" />.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Converts the <see cref="Contracts.EventHorizonEvent" /> to <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Contracts.EventHorizonEvent" />.</param>
        /// <param name="producerMicroservice">The producer <see cref="Microservice" />.</param>
        /// <param name="consumerTenant">The consumer <see cref="TenantId" />.</param>
        /// <returns>The <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this Contracts.EventHorizonEvent @event, Microservice producerMicroservice, TenantId consumerTenant) =>
            new CommittedEvent(
                uint.MaxValue, // TODO: Origin event log sequence number here.
                @event.Occurred.ToDateTimeOffset(),
                @event.EventSourceId.To<EventSourceId>(),
                new ExecutionContext(
                    producerMicroservice,
                    consumerTenant,
                    Versioning.Version.NotSet,
                    Environment.Undetermined,
                    @event.CorrelationId.To<CorrelationId>(),
                    Claims.Empty,
                    CultureInfo.InvariantCulture),
                new Artifact(@event.Type.Id.To<ArtifactId>(), @event.Type.Generation),
                false,
                @event.Content);
    }
}