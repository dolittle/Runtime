// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.EventHorizon.Consumer;

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
    /// Increments the current number of connected subscriptions.
    /// </summary>
    void IncrementCurrentConnectedSubscriptions();

    /// <summary>
    /// Decrements the current number of connected subscriptions.
    /// </summary>
    void DecrementCurrentConnectedSubscriptions();

    /// <summary>
    /// Increments the total number of subscriptions that have already been started.
    /// </summary>
    void IncrementSubscriptionsAlreadyStarted();

    /// <summary>
    /// Increments the total number of subscriptions where there is a missing producer microservice address configuration.
    /// </summary>
    void IncrementSubscriptionsMissingProducerMicroserviceAddress();

    /// <summary>
    /// Increments the total number of subscriptions that has failed due to an exception.
    /// </summary>
    void IncrementSubscriptionsFailedDueToException();

    /// <summary>
    /// Increments the total number of subscriptions that has failed due to one either receiving or writign events completing.
    /// </summary>
    void IncrementSubscriptionsFailedDueToReceivingOrWritingEventsCompleted();

    /// <summary>
    /// Increments the total number of subscriptions loop roundtrips.
    /// </summary>
    void IncrementSubscriptionLoops();
}