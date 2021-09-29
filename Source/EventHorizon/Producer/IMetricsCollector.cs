// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Defines a system for collecting metrics about event horizon consumer.
    /// </summary>
    public interface IMetricsCollector
    {
        /// <summary>
        /// Increments the total number of incoming subscriptions from another Runtime.
        /// </summary>
        void IncrementTotalIncomingSubscriptions();

        /// <summary>
        /// Increments the total number of rejected subscriptions from another Runtime.
        /// </summary>
        void IncrementTotalRejectedSubscriptions();

        /// <summary>
        /// Increments the total number of rejected subscriptions from another Runtime.
        /// </summary>
        void IncrementTotalAcceptedSubscriptions();

        /// <summary>
        /// Increments the total number of events written to event horizon.
        /// </summary>
        void IncrementTotalEventsWrittenToEventHorizon();
    }
}
