// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when a tenant is configured to have multiple subscriptions to the same tenant in a microservice.
    /// </summary>
    public class TenantHasMultipleSubscriptionsToSameTenantInMicroservice : InvalidEventHorizonSubscriptionsConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantHasMultipleSubscriptionsToSameTenantInMicroservice"/> class.
        /// </summary>
        /// <param name="subscriberTenant">The subscriber <see cref="TenantId" />.</param>
        /// <param name="microservice">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        public TenantHasMultipleSubscriptionsToSameTenantInMicroservice(TenantId subscriberTenant, Microservice microservice, TenantId producerTenant)
            : base($"Tenant '{subscriberTenant}' is configured to subscribe to tenant '{producerTenant}' in microservice '{microservice}' multiple times.")
        {
        }
    }
}