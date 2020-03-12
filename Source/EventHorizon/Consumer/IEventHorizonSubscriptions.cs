// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system that knows about <see cref="EventHorizonSubscription" />.
    /// </summary>
    public interface IEventHorizonSubscriptions
    {
        /// <summary>
        /// Gets the <see cref="EventHorizonSubscription" /> for the subscriber has to the given tenant in a microservice.
        /// </summary>
        /// <param name="subscriberTenant">The subscriber <see cref="TenantId" />.</param>
        /// <param name="producerMicroservice">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        /// <returns>The <see cref="EventHorizonSubscription" />.</returns>
        EventHorizonSubscription GetSubscriptionFor(TenantId subscriberTenant, Microservice producerMicroservice, TenantId producerTenant);
    }
}