// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, SubscriptionId, Exception> _alreadySubscribedTo = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Debug,
                new EventId(935150893, nameof(AlreadySubscribedTo)),
                "Already subscribed to subscription {SubscriptionId}");
        
        static readonly Action<ILogger, Guid, Exception> _noMicroserviceConfigurationFor = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(371825462, nameof(NoMicroserviceConfigurationFor)),
                "There is no microservice configuration for the producer microservice {ProducerMicrosrevice}");

        static readonly Action<ILogger, Guid, Guid, Guid, string, int, Exception> _tenantSubscribedTo = LoggerMessage
            .Define<Guid, Guid, Guid, string, int>(
                LogLevel.Debug,
                new EventId(35626265, nameof(TenantSubscribedTo)),
                "Tenant '{ConsumerTenantId}' is subscribing to events from tenant '{ProducerTenantId}' in microservice '{ProducerMicroserviceId}' on address '{Host}:{Port}'");
        static readonly Action<ILogger, SubscriptionId, Exception> _didNotReceiveSubscriptionResponse = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(161145735, nameof(DidNotReceiveSubscriptionResponse)),
                "Reverse call client did not receive a subscription response while subscribing {Subscription}");

        static readonly Action<ILogger, SubscriptionId, FailureReason, Exception> _failedSubscring = LoggerMessage
            .Define<SubscriptionId, FailureReason>(
                LogLevel.Warning,
                new EventId(265824309, nameof(FailedSubscring)),
                "Failed subscribing with subscription {SubscriptionId}. {Reason}");

        static readonly Action<ILogger, SubscriptionId, Exception> _successfulSubscring = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(1573331110, nameof(SuccessfulSubscring)),
                "Subscription response for subscription {SubscriptionId} was successful");

        static readonly Action<ILogger, SubscriptionId, Exception> _reconnectingEventHorizon = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Debug,
                new EventId(382210240, nameof(ReconnectingEventHorizon)),
                "Reconnecting to event horizon with subscription {SubscriptionId}");

        static readonly Action<ILogger, SubscriptionId, Exception> _connectedEventHorizon = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Information,
                new EventId(304788361, nameof(ConnectedEventHorizon)),
                "Successfully connected event horizon with {SubscriptionId}. Waiting for events to process");

        static readonly Action<ILogger, SubscriptionId, Exception> _errorInitializingSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(294838925, nameof(ErrorInitializingSubscription)),
                "Error occurred while initializing Subscription: {SubscriptionId}");

        static readonly Action<ILogger, SubscriptionId, Exception> _errorWhileProcessingSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(1328073767, nameof(ErrorWhileProcessingSubscription)),
                "Error occurred while processing Subscription: {SubscriptionId}");

        static readonly Action<ILogger, SubscriptionId, Exception> _errorWhileHandlingEventFromSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(829474082, nameof(ErrorWhileHandlingEventFromSubscription)),
                "An error occurred while handling event horizon event coming from subscription {Subscription}");
        
        internal static void AlreadySubscribedTo(this ILogger logger, SubscriptionId subscriptionId)
            => _alreadySubscribedTo(logger, subscriptionId, null);

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

    }
}