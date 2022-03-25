// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer;

static class LoggerExtensions
{

    #region Subscriptions
    static readonly Action<ILogger, SubscriptionId, Exception> _subscribingTo = LoggerMessage
        .Define<SubscriptionId>(
            LogLevel.Debug,
            new EventId(296984987, nameof(SubscribingTo)),
            "Subscribing to events for subscription {Subscription}");
    internal static void SubscribingTo(this ILogger logger, SubscriptionId subscriptionId)
        => _subscribingTo(logger, subscriptionId, null);

    static readonly Action<ILogger, Guid, Exception> _noMicroserviceConfigurationFor = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(371825462, nameof(NoMicroserviceConfigurationFor)),
            "There is no microservice configuration for the producer microservice: {ProducerMicroservice}");
    internal static void NoMicroserviceConfigurationFor(this ILogger logger, MicroserviceId producerMicroservice)
        => _noMicroserviceConfigurationFor(logger, producerMicroservice, null);

    static readonly Action<ILogger, SubscriptionId, Exception> _startingCreatedSubscription = LoggerMessage
        .Define<SubscriptionId>(
            LogLevel.Debug,
            new EventId(251473054, nameof(StartingCreatedSubscription)),
            "Starting newly created subscription for subscription {Subscription}");
    internal static void StartingCreatedSubscription(this ILogger logger, SubscriptionId subscriptionId)
        => _startingCreatedSubscription(logger, subscriptionId, null);

    static readonly Action<ILogger, SubscriptionId, Exception> _subscriptionAlreadyRegistered = LoggerMessage
        .Define<SubscriptionId>(
            LogLevel.Debug,
            new EventId(34552119, nameof(SubscriptionAlreadyRegistered)),
            "Subscription already registered for subscription {Subscription}");
    internal static void SubscriptionAlreadyRegistered(this ILogger logger, SubscriptionId subscriptionId)
        => _subscriptionAlreadyRegistered(logger, subscriptionId, null);


    #endregion

    #region SubscriptionPolicy

    static readonly Action<ILogger, Exception> _subscriptionConnectionFailed = LoggerMessage
        .Define(
            LogLevel.Warning,
            new EventId(669934152, nameof(SubscriptionConnectionFailed)),
            "Subscription connection failed with exception, policy will restart a new connection");
    internal static void SubscriptionConnectionFailed(this ILogger logger, Exception exception)
        => _subscriptionConnectionFailed(logger, exception);

    #endregion

    #region Subscription

    static readonly Action<ILogger, SubscriptionState, SubscriptionId, Exception> _subscriptionAlreadyStarted = LoggerMessage
        .Define<SubscriptionState, SubscriptionId>(
            LogLevel.Trace,
            new EventId(606555688, nameof(SubscriptionAlreadyStarted)),
            "Subscription is already started and in state {SubscriptionState} for subscription {Subscription}");
    internal static void SubscriptionAlreadyStarted(this ILogger logger, SubscriptionId subscriptionId, SubscriptionState state)
        => _subscriptionAlreadyStarted(logger, state, subscriptionId, null);

    static readonly Action<ILogger, SubscriptionId, Exception> _subscriptionStarting = LoggerMessage
        .Define<SubscriptionId>(
            LogLevel.Debug,
            new EventId(794823441, nameof(SubscriptionStarting)),
            "Subscription starting for subscription {Subscription}");
    internal static void SubscriptionStarting(this ILogger logger, SubscriptionId subscriptionId)
        => _subscriptionStarting(logger, subscriptionId, null);

    static readonly Action<ILogger, SubscriptionState, SubscriptionId, Exception> _subscriptionLoopStarting = LoggerMessage
        .Define<SubscriptionState, SubscriptionId>(
            LogLevel.Debug,
            new EventId(606555688, nameof(SubscriptionLoopStarting)),
            "Subscription loop starting from previous state {PreviousState} for subscription {Subscription}");
    internal static void SubscriptionLoopStarting(this ILogger logger, SubscriptionId subscriptionId, SubscriptionState previousState)
        => _subscriptionLoopStarting(logger, previousState, subscriptionId, null);

    static readonly Action<ILogger, Failure, SubscriptionId, Exception> _subscriptionFailedToConnectToProducerRuntime = LoggerMessage
        .Define<Failure, SubscriptionId>(
            LogLevel.Warning,
            new EventId(720998740, nameof(SubscriptionFailedToConnectToProducerRuntime)),
            "Subscription failed to connect to producer runtime because {FailureReason} for subscription {Subscription}");
    internal static void SubscriptionFailedToConnectToProducerRuntime(this ILogger logger, SubscriptionId subscriptionId, Failure failure)
        => _subscriptionFailedToConnectToProducerRuntime(logger, failure, subscriptionId, null);

    static readonly Action<ILogger, SubscriptionId, Exception> _subscriptionFailedWithException = LoggerMessage
        .Define<SubscriptionId>(
            LogLevel.Warning,
            new EventId(426941604, nameof(SubscriptionAlreadyStarted)),
            "Subscription failed with exception for subscription {Subscription}");
    internal static void SubscriptionFailedWithException(this ILogger logger, SubscriptionId subscriptionId, Exception exception)
        => _subscriptionFailedWithException(logger, subscriptionId, exception);

    static readonly Action<ILogger, Guid, SubscriptionId, Exception> _subscriptionIsReceivingAndWriting = LoggerMessage
        .Define<Guid, SubscriptionId>(
            LogLevel.Debug,
            new EventId(556421021, nameof(SubscriptionIsReceivingAndWriting)),
            "Subscription is receiving and writing events with consent {Consent} for subscription {Subscription}");
    internal static void SubscriptionIsReceivingAndWriting(this ILogger logger, SubscriptionId subscriptionId, ConsentId consentId)
        => _subscriptionIsReceivingAndWriting(logger, consentId, subscriptionId, null);

    static readonly Action<ILogger, Guid, SubscriptionId, Exception> _subsciptionFailedWhileReceivingAndWriting = LoggerMessage
        .Define<Guid, SubscriptionId>(
            LogLevel.Warning,
            new EventId(254259038, nameof(SubsciptionFailedWhileReceivingAndWriting)),
            "Subscription failed while receiving and writing events with consent {Consent} for subscription {Subscription}");
    internal static void SubsciptionFailedWhileReceivingAndWriting(this ILogger logger, SubscriptionId subscriptionId, ConsentId consentId, Exception exception)
        => _subsciptionFailedWhileReceivingAndWriting(logger, consentId, subscriptionId, exception);

    #endregion

    #region SubscriptionsService
    static readonly Action<ILogger, SubscriptionId, Exception> _incomingSubscripton = LoggerMessage
        .Define<SubscriptionId>(
            LogLevel.Information,
            new EventId(409082696, nameof(IncomingSubscripton)),
            "Incoming event horizon subscription for subscription {Subscription}");
    internal static void IncomingSubscripton(this ILogger logger, SubscriptionId subscriptionId)
        => _incomingSubscripton(logger, subscriptionId, null);

    static readonly Action<ILogger, SubscriptionId, Exception> _errorWhileSubscribing = LoggerMessage
        .Define<SubscriptionId>(
            LogLevel.Warning,
            new EventId(1317844869, nameof(ErrorWhileSubscribing)),
            "An error occurred while trying to handling event horizon subscription for {Subscription}");
    internal static void ErrorWhileSubscribing(this ILogger logger, SubscriptionId subscriptionId, Exception exception)
        => _errorWhileSubscribing(logger, subscriptionId, exception);
    #endregion
}
