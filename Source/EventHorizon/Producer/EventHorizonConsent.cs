// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents the consent for an event horizon.
    /// </summary>
    public class EventHorizonConsent
    {
        /// <summary>
        /// Gets or sets the <see cref="Microservice" /> to give consent to.
        /// </summary>
        public Microservice Microservice { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId" /> tenant to give consent to.
        /// </summary>
        public TenantId Tenant { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="StreamId" /> stream to give consent to.
        /// </summary>
        public StreamId Stream { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PartitionId" /> partition in the stream to give consent to.
        /// </summary>
        public PartitionId Partition { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventHorizonConsentId" /> for the tenant in microservice.
        /// </summary>
        public EventHorizonConsentId Consent { get; set; }
    }
}