// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections
{
    /// <summary>
    /// Defines a system for collecting metrics about event horizon consumer.
    /// </summary>
    public interface IMetricsCollector
    {
        /// <summary>
        /// Increments the total number of connection attempts made.
        /// </summary>
        void IncrementTotalConnectionAttempts();

        /// <summary>
        /// Increments the total number of connection attempts failed.
        /// </summary>
        void IncrementTotalConnectionsFailed();

        /// <summary>
        /// Increments the total number of successful connection responses.
        /// </summary>
        void IncrementTotalSuccessfulResponses();

        /// <summary>
        /// Increments the total number of failure connection responses.
        /// </summary>
        void IncrementTotalFailureResponses();

        /// <summary>
        /// Increments the total number of event horizon events handled.
        /// </summary>
        void IncrementTotalEventHorizonEventsHandled();

        /// <summary>
        /// Increments the total number of event horizon events failed handling.
        /// </summary>
        void IncrementTotalEventHorizonEventsFailedHandling();

        /// <summary>
        /// Adds to the total time spent connecting to an event horizon.
        /// </summary>
        /// <param name="elapsed">The timespan élapsed successfully connecting to an event horizon.</param>
        void AddTotalTimeSpentConnecting(TimeSpan elapsed);

        /// <summary>
        /// Increments the total number of subscriptions requests to a Producer Runtime where the request was missing arguments.
        /// </summary>
        void IncrementTotalSubcriptionsWithMissingArguments();

        /// <summary>
        /// Increments the total number of subscription requests to a Producer Runtime where there was no consent.
        /// </summary>
        void IncrementTotalSubscriptionsWithMissingConsent();
    }
}
