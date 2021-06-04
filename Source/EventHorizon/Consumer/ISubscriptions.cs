// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system that keeps track of event horizon subscriptions.
    /// </summary>
    public interface ISubscriptions
    {
        /// <summary>
        /// Adds an event horizon subscription if one isn't already present.
        /// </summary>
        /// <param name="consentId">The <see cref="ConsentId" />.</param>
        /// <param name="subscriptionId">The <see cref="SubscriptionId"/>.</param>
        /// <param name="eventProcessor">The <see cref="EventProcessor" />.</param>
        /// <param name="eventsFetcher">The <see cref="EventsFromEventHorizonFetcher" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <param name="subscription">The <see cref="Subscription" />.</param>
        /// <returns>A <see cref="SubscriptionResponse"/>.</returns>
        Task<SubscriptionResponse> Subscribe(SubscriptionId subscriptionId, CancellationToken cancellationToken);
    }
}
