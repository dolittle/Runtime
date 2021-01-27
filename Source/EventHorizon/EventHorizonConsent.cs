// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents the consent for an event horizon.
    /// </summary>
    public record EventHorizonConsent
    {
        /// <summary>
        /// Gets or sets the <see cref="Microservice" /> to give consent to.
        /// </summary>
        public Microservice Microservice { get; init; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId" /> tenant to give consent to.
        /// </summary>
        public TenantId Tenant { get; init; }

        /// <summary>
        /// Gets or sets the <see cref="StreamId" /> stream to give consent to.
        /// </summary>
        public StreamId Stream { get; init; }

        /// <summary>
        /// Gets or sets the <see cref="PartitionId" /> partition in the stream to give consent to.
        /// </summary>
        public PartitionId Partition { get; init; }

        /// <summary>
        /// Gets or sets the <see cref="ConsentId" /> for the tenant in microservice.
        /// </summary>
        public ConsentId Consent { get; init; }
    }
    /// <summary>
    /// Represents the consent configuration for an event horizon.
    /// </summary>
    public record EventHorizonConsentConfiguration(Guid Microservice, Guid Tenant, Guid Stream, Guid Partition, Guid Consent)
    {
        public static implicit operator EventHorizonConsent(EventHorizonConsentConfiguration config)
            => new()
            {
                Microservice = config.Microservice,
                Tenant = config.Tenant,
                Stream = config.Stream,
                Partition = config.Partition,
                Consent = config.Consent
            };
    }
}
