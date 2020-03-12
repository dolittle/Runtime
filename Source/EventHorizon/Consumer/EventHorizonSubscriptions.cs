// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizonSubscriptions" />.
    /// </summary>
    public class EventHorizonSubscriptions : IEventHorizonSubscriptions
    {
        readonly EventHorizonSubscriptionsConfiguration _subscriptionsConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonSubscriptions"/> class.
        /// </summary>
        /// <param name="subscriptionsConfiguration">The <see cref="EventHorizonSubscriptionsConfiguration" />.</param>
        public EventHorizonSubscriptions(EventHorizonSubscriptionsConfiguration subscriptionsConfiguration) => _subscriptionsConfiguration = subscriptionsConfiguration;

        /// <inheritdoc/>
        public EventHorizonSubscription GetSubscriptionFor(TenantId subscriberTenant, Microservice producerMicroservice, TenantId producerTenant)
        {
            if (!_subscriptionsConfiguration.TryGetValue(subscriberTenant, out var subscriptions)) throw new NoSubscriptionsForSubscriberTenant(subscriberTenant);
            var matchingSubscriptions = subscriptions.Where(_ => _.Microservice == producerMicroservice && _.Tenant == producerTenant);
            if (matchingSubscriptions.Count() > 1) throw new TenantHasMultipleSubscriptionsToSameTenantInMicroservice(subscriberTenant, producerMicroservice, producerTenant);
            if (!matchingSubscriptions.Any()) throw new TenantHasNoSubscriptionForTenantInMicroservice(subscriberTenant, producerMicroservice, producerTenant);

            return matchingSubscriptions.First();
        }
    }
}