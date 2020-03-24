// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Exception that is thrown when a given tenant has no consent configured for a tenant in a microservice.
    /// </summary>
    public class TenantHasNoConsentForTenantInMicroservice : InvalidEventHorizonConsentsConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantHasNoConsentForTenantInMicroservice"/> class.
        /// </summary>
        /// <param name="publisherTenant">The publisher <see cref="TenantId" />.</param>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <param name="subscriberTenant">The subscriber <see cref="TenantId" />.</param>
        /// <param name="publicStream">The public <see cref="StreamId" />.</param>
        /// <param name="publicStreamPartition">The <see cref="PartitionId" /> in the public stream.</param>
        public TenantHasNoConsentForTenantInMicroservice(TenantId publisherTenant, Microservice microservice, TenantId subscriberTenant, StreamId publicStream, PartitionId publicStreamPartition)
            : base($"The tenant '{publisherTenant}' does not have any event horizon consents configured for tenant '{subscriberTenant}' in microservice '{microservice}' to partition '{publicStreamPartition}' in public stream '{publicStream}'")
        {
        }
    }
}