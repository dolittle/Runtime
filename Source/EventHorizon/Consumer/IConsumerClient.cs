// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system for handling inncomming public events from other microservices.
    /// </summary>
    public interface IConsumerClient
    {
        /// <summary>
        /// Handles a subscription request.
        /// </summary>
        /// <param name="subscription">The <see cref="Subscription" />.</param>
        /// <returns>The <see cref="SubscriptionResponse" />.</returns>
        Task<SubscriptionResponse> HandleSubscription(Subscription subscription);
    }
}