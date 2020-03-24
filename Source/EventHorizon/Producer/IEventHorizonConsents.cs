// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Applications;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Defines a system that knows about <see cref="EventHorizonConsentsConfiguration" />.
    /// </summary>
    public interface IEventHorizonConsents
    {
        /// <summary>
        /// Gets the <see cref="EventHorizonConsentConfiguration" /> from publisher <see cref="TenantId" /> towards a tenant in another microservice.
        /// </summary>
        /// <param name="publisherTenant">The publisher <see cref="TenantId" />.</param>
        /// <param name="subscriberMicroservice">The subscriber <see cref="Microservice" />.</param>
        /// <param name="subscriberTenant">The subscriber <see cref="TenantId" />.</param>
        /// <param name="publicStream">The public stream to subscribe to <see cref="StreamId" />.</param>
        /// <param name="publicStreamPartition">The <see cref="PartitionId" /> in the public stream to subscribe to.</param>
        /// <returns>The <see cref="EventHorizonConsentConfiguration" />.</returns>
        EventHorizonConsent GetConsentFor(TenantId publisherTenant, Microservice subscriberMicroservice, TenantId subscriberTenant, StreamId publicStream, PartitionId publicStreamPartition);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}" /> list of <see cref="EventHorizonConsentConfiguration" />.
        /// </summary>
        /// <param name="publisherTenant">The publisher <see cref="TenantId" />.</param>
        /// <returns>The <see cref="IEnumerable{T}" /> list of <see cref="EventHorizonConsentConfiguration" /> for a tenant.</returns>
        IEnumerable<EventHorizonConsentConfiguration> GetConsentConfigurationsFor(TenantId publisherTenant);
    }
}