// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.EventHorizon.Producer;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system that keeps track of event horizon subscriptions.
    /// </summary>
    public interface ISubscriptions
    {
        /// <summary>
        /// Adds an event horizon subscription.
        /// </summary>
        /// <param name="consentId">The <see cref="EventHorizonConsentId" />.</param>
        /// <param name="subscriptionId">The <see cref="SubscriptionId"/>.</param>
        /// <param name="eventProcessor">The <see cref="EventProcessor" />.</param>
        /// <param name="eventsFetcher">The <see cref="EventsFromEventHorizonFetcher" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <param name="subscription">The <see cref="Subscription" />.</param>
        /// <returns>False if the Subscription already exists, true if not.</returns>
        bool TrySubscribe(EventHorizonConsentId consentId, SubscriptionId subscriptionId, EventProcessor eventProcessor, EventsFromEventHorizonFetcher eventsFetcher, CancellationToken cancellationToken, out Subscription subscription);

        /// <summary>
        /// Tries to get the <see cref="EventHorizonConsentId "/> for a <see cref="SubscriptionId" />.
        /// </summary>
        /// <param name="subscriptionId">The <see cref="SubscriptionId" />.</param>
        /// <param name="consentId">The <see cref="EventHorizonConsentId" />.</param>
        /// <returns>A value indicating whether the subscription exists or not.</returns>
        bool TryGetConsentFor(SubscriptionId subscriptionId, out EventHorizonConsentId consentId);
    }
}
