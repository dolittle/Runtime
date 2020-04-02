// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        /// <param name="subscription">The <see cref="Subscription "/>.</param>
        /// <returns>False if the subscription already exists, false if not.</returns>
        bool AddSubscription(Subscription subscription);
    }
}
