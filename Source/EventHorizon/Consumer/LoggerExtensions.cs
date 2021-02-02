// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    internal static class LoggerExtensions
    {   
        static readonly Action<ILogger, Guid, Exception> _noMicroserviceConfigurationFor = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(371825462, nameof(NoMicroserviceConfigurationFor)),
                "There is no microservice configuration for the producer microservice: {ProducerMicrosrevice}");

        static readonly Action<ILogger, Guid, Guid, Guid, string, int, Exception> _tenantSubscribedTo = LoggerMessage
            .Define<Guid, Guid, Guid, string, int>(
                LogLevel.Debug,
                new EventId(35626265, nameof(TenantSubscribedTo)),
                "Tenant: {ConsumerTenantId} is subscribing to events from tenant: {ProducerTenantId} in microservice: {ProducerMicroserviceId} on {Host}:{Port}");
        static readonly Action<ILogger, SubscriptionId, Exception> _didNotReceiveSubscriptionResponse = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(161145735, nameof(DidNotReceiveSubscriptionResponse)),
                "Reverse call client did not receive a subscription response while subscribing: {Subscription}");

        static readonly Action<ILogger, SubscriptionId, FailureReason, Exception> _failedSubscring = LoggerMessage
            .Define<SubscriptionId, FailureReason>(
                LogLevel.Warning,
                new EventId(265824309, nameof(FailedSubscring)),
                "Failed subscribing with subscription: {SubscriptionId}. {Reason}");

        static readonly Action<ILogger, SubscriptionId, Exception> _successfulSubscring = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(1573331110, nameof(SuccessfulSubscring)),
                "Subscription response for subscription: {SubscriptionId} was successful");

        static readonly Action<ILogger, SubscriptionId, Exception> _reconnectingEventHorizon = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Debug,
                new EventId(382210240, nameof(ReconnectingEventHorizon)),
                "Reconnecting to event horizon with subscription: {SubscriptionId}");

        static readonly Action<ILogger, SubscriptionId, Exception> _connectedEventHorizon = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Information,
                new EventId(304788361, nameof(ConnectedEventHorizon)),
                "Successfully connected event horizon with subscription: {SubscriptionId}.\nWaiting for events to process");

        static readonly Action<ILogger, SubscriptionId, Exception> _errorInitializingSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(294838925, nameof(ErrorInitializingSubscription)),
                "Error occurred while initializing subscription: {SubscriptionId}");

        static readonly Action<ILogger, SubscriptionId, Exception> _errorWhileProcessingSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(1328073767, nameof(ErrorWhileProcessingSubscription)),
                "Error occurred while processing subscription: {SubscriptionId}");

        static readonly Action<ILogger, SubscriptionId, Exception> _errorWhileHandlingEventFromSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(829474082, nameof(ErrorWhileHandlingEventFromSubscription)),
                "An error occurred while handling event horizon event coming from subscription: {Subscription}");

        static readonly Action<ILogger, SubscriptionId, Exception> _retryProcessEvent = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(315586919, nameof(RetryProcessEvent)),
                "Retrying processing of event from Event Horizon for subscription: {Subscription}");

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Exception> _processEvent = LoggerMessage
            .Define<Guid, Guid, Guid, Guid>(
                LogLevel.Trace,
                new EventId(1329962982, nameof(ProcessEvent)),
                "Processing event: {EventType} from event horizon in scope: {Scope} from microservice: {ProducerMicroservice} and tenant: {ProducerTenant}");

        static readonly Action<ILogger, SubscriptionId, Exception> _failedStartingSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(227530761, nameof(FailedStartingSubscription)),
                "Subscription: {SubscriptionId} failed");

        static readonly Action<ILogger, SubscriptionId, Exception> _subscriptionAlreadyRegistered = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(34552119, nameof(SubscriptionAlreadyRegistered)),
                "Subscription: {SubscriptionId} already registered");

        static readonly Action<ILogger, SubscriptionId, Exception> _successfullyRegisteredSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(92493948, nameof(SuccessfullyRegisteredSubscription)),
                "Subscription: {SubscriptionId} successfully registered");

        static readonly Action<ILogger, SubscriptionId, Exception> _unregisteringSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Debug,
                new EventId(908025101, nameof(UnregisteringSubscription)),
                "Unregistering subscription: {SubscriptionId}");

        static readonly Action<ILogger, SubscriptionId, Exception> _incomingSubscripton = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Information,
                new EventId(409082696, nameof(IncomingSubscripton)),
                "Incoming event horizon subscription request from head to runtime for subscription: {SubscriptionId}");

        static readonly Action<ILogger, SubscriptionId, Exception> _errorWhileSubscribing = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(1317844869, nameof(ErrorWhileSubscribing)),
                "An error occurred while trying to handling event horizon subscription: {Subscription}");

        internal static void NoMicroserviceConfigurationFor(this ILogger logger, Microservice producerMicroservice)
            => _noMicroserviceConfigurationFor(logger, producerMicroservice, null);

        internal static void TenantSubscribedTo(this ILogger logger, TenantId consumerTenant, TenantId producerTenant, Microservice producerMicroservice, MicroserviceAddress microserviceAddress)
            => _tenantSubscribedTo(logger, consumerTenant, producerTenant, producerMicroservice, microserviceAddress.Host, microserviceAddress.Port, null);
        internal static void DidNotReceiveSubscriptionResponse(this ILogger logger, SubscriptionId subscription)
            => _didNotReceiveSubscriptionResponse(logger, subscription, null);

        internal static void FailedSubscring(this ILogger logger, SubscriptionId subscription, FailureReason reason)
            => _failedSubscring(logger, subscription, reason, null);

        internal static void SuccessfulSubscring(this ILogger logger, SubscriptionId subscription)
            => _successfulSubscring(logger, subscription, null);

        internal static void ReconnectingEventHorizon(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _reconnectingEventHorizon(logger, subscription, exception);

        internal static void ConnectedEventHorizon(this ILogger logger, SubscriptionId subscription)
            => _connectedEventHorizon(logger, subscription, null);

        internal static void ErrorInitializingSubscription(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _errorInitializingSubscription(logger, subscription, exception);

        internal static void ErrorWhileProcessingSubscription(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _errorWhileProcessingSubscription(logger, subscription, exception);

        internal static void ErrorWhileHandlingEventFromSubscription(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _errorWhileHandlingEventFromSubscription(logger, subscription, exception);

        internal static void RetryProcessEvent(this ILogger logger, SubscriptionId subscription)
            => _retryProcessEvent(logger, subscription, null);

        internal static void ProcessEvent(this ILogger logger, ArtifactId eventTypeId, ScopeId scope, Microservice producerMicroservice, TenantId producerTenant)
            => _processEvent(logger, eventTypeId, scope, producerMicroservice, producerTenant, null);

        internal static void FailedStartingSubscription(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _failedStartingSubscription(logger, subscription, exception);

        internal static void SubscriptionAlreadyRegistered(this ILogger logger, SubscriptionId subscription)
            => _subscriptionAlreadyRegistered(logger, subscription, null);

        internal static void SuccessfullyRegisteredSubscription(this ILogger logger, SubscriptionId subscription)
            => _successfullyRegisteredSubscription(logger, subscription, null);
        
        internal static void UnregisteringSubscription(this ILogger logger, SubscriptionId subscription)
            => _unregisteringSubscription(logger, subscription, null);

        internal static void IncomingSubscripton(this ILogger logger, SubscriptionId subscription)
            => _incomingSubscripton(logger, subscription, null);

        internal static void ErrorWhileSubscribing(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _errorWhileSubscribing(logger, subscription, exception);
    }
}
