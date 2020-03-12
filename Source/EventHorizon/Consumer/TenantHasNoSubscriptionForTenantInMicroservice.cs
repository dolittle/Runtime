// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when a tenant does not have a subscription towards a given tenant in a microservice.
    /// </summary>
    public class TenantHasNoSubscriptionForTenantInMicroservice : InvalidEventHorizonSubscriptionsConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantHasNoSubscriptionForTenantInMicroservice"/> class.
        /// </summary>
        /// <param name="subscriberTenant">The subscriber <see cref="TenantId" />.</param>
        /// <param name="microservice">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        public TenantHasNoSubscriptionForTenantInMicroservice(TenantId subscriberTenant, Microservice microservice, TenantId producerTenant)
            : base($"Tenant '{subscriberTenant}' does not have a subscription to tenant '{producerTenant}' in microservice '{microservice}'.")
        {
        }
    }
}