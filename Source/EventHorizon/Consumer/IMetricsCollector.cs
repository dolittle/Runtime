// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system for collecting metrics about event horizon consumer.
    /// </summary>
    public interface IMetricsCollector
    {
        /// <summary>
        /// Increments the total number of subscription requests received from Head.
        /// </summary>
        void IncrementTotalSubscriptionsInitiatedFromHead();

        /// <summary>
        /// Increments the total number of registered subscriptions.
        /// </summary>
        void IncrementTotalRegisteredSubscriptions();

        /// <summary>
        /// Increments the total number of connected subscriptions.
        /// </summary>
        void IncrementTotalConnectedSubscriptions();

        /// <summary>
        /// Decrements the total number of connected subscriptions.
        /// </summary>
        void DecrementTotalConnectedSubscriptions();

    }
}
