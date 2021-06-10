// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    }
}
