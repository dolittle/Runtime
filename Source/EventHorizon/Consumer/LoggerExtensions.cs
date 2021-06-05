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

#region Subscriptions
        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Guid, Exception> _subscribingTo = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Debug,
                new EventId(296984987, nameof(SubscribingTo)),
                "Subscribing to events for: tenant {ConsumerTenant} from partition {Partition} in stream {Stream} from tenant {ProducerTenant} in microservice {ProducerMicroservice} into scope {Scope}");
        internal static void SubscribingTo(this ILogger logger, SubscriptionId subscriptionId)
            => _subscribingTo(logger, subscriptionId.ConsumerTenantId, subscriptionId.PartitionId, subscriptionId.StreamId, subscriptionId.ProducerTenantId, subscriptionId.ProducerMicroserviceId, subscriptionId.ScopeId, null);

        static readonly Action<ILogger, Guid, Exception> _noMicroserviceConfigurationFor = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(371825462, nameof(NoMicroserviceConfigurationFor)),
                "There is no microservice configuration for the producer microservice: {ProducerMicrosrevice}");
        internal static void NoMicroserviceConfigurationFor(this ILogger logger, Microservice producerMicroservice)
            => _noMicroserviceConfigurationFor(logger, producerMicroservice, null);

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Guid, Exception> _startingCreatedSubscription = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Debug,
                new EventId(251473054, nameof(StartingCreatedSubscription)),
                "Starting newly created subscription for: tenant {ConsumerTenant} from partition {Partition} in stream {Stream} from tenant {ProducerTenant} in microservice {ProducerMicroservice} into scope {Scope}");
        internal static void StartingCreatedSubscription(this ILogger logger, SubscriptionId subscriptionId)
            => _startingCreatedSubscription(logger, subscriptionId.ConsumerTenantId, subscriptionId.PartitionId, subscriptionId.StreamId, subscriptionId.ProducerTenantId, subscriptionId.ProducerMicroserviceId, subscriptionId.ScopeId, null);

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Guid, Exception> _subscriptionAlreadyRegistered = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Debug,
                new EventId(34552119, nameof(SubscriptionAlreadyRegistered)),
                "Subscription already started for: tenant {ConsumerTenant} from partition {Partition} in stream {Stream} from tenant {ProducerTenant} in microservice {ProducerMicroservice} into scope {Scope}");
        internal static void SubscriptionAlreadyRegistered(this ILogger logger, SubscriptionId subscriptionId)
            => _subscriptionAlreadyRegistered(logger, subscriptionId.ConsumerTenantId, subscriptionId.PartitionId, subscriptionId.StreamId, subscriptionId.ProducerTenantId, subscriptionId.ProducerMicroserviceId, subscriptionId.ScopeId, null);


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


#region EventHorizonConnection

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Guid, Exception> _connectionToProducerRuntimeSucceeded = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Trace,
                new EventId(1573331110, nameof(ConnectionToProducerRuntimeSucceeded)),
                "Connection to producer runtime succeeded for subscription: tenant {ConsumerTenant} from partition {Partition} in stream {Stream} from tenant {ProducerTenant} in microservice {ProducerMicroservice} into scope {Scope}");
        internal static void ConnectionToProducerRuntimeSucceeded(this ILogger logger, SubscriptionId subscriptionId)
            => _connectionToProducerRuntimeSucceeded(logger, subscriptionId.ConsumerTenantId, subscriptionId.PartitionId, subscriptionId.StreamId, subscriptionId.ProducerTenantId, subscriptionId.ProducerMicroserviceId, subscriptionId.ScopeId, null);

        static readonly Action<ILogger, string, Guid, Guid, Guid, Guid, Guid, Exception> _connectionToProducerRuntimeFailed = LoggerMessage
            .Define<string, Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Warning,
                new EventId(265824309, nameof(ConnectionToProducerRuntimeFailed)),
                "Connection to producer Runtime failed because {Reason} for subscription: tenant {ConsumerTenant} from partition {Partition} in stream {Stream} from tenant {ProducerTenant} in microservice {ProducerMicroservice}");
        internal static void ConnectionToProducerRuntimeFailed(this ILogger logger, SubscriptionId subscriptionId, FailureReason reason)
            => _connectionToProducerRuntimeFailed(logger, reason, subscriptionId.ConsumerTenantId, subscriptionId.PartitionId, subscriptionId.StreamId, subscriptionId.ProducerTenantId, subscriptionId.ProducerMicroserviceId, null);

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Guid, Exception> _couldNotConnectToProducerRuntime = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Trace,
                new EventId(305416696, nameof(CouldNotConnectToProducerRuntime)),
                "Could not connect to producer Runtime for subscription: tenant {ConsumerTenant} from partition {Partition} in stream {Stream} from tenant {ProducerTenant} in microservice {ProducerMicroservice} into scope {Scope}");
        internal static void CouldNotConnectToProducerRuntime(this ILogger logger, SubscriptionId subscriptionId)
            => _couldNotConnectToProducerRuntime(logger, subscriptionId.ConsumerTenantId, subscriptionId.PartitionId, subscriptionId.StreamId, subscriptionId.ProducerTenantId, subscriptionId.ProducerMicroserviceId, subscriptionId.ScopeId, null);

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Guid, Exception> _handlingEventForSubscription = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Trace,
                new EventId(844153427, nameof(HandlingEventForSubscription)),
                "Handling event horizon event for subscription: tenant {ConsumerTenant} from partition {Partition} in stream {Stream} from tenant {ProducerTenant} in microservice {ProducerMicroservice} into scope {Scope}");
        internal static void HandlingEventForSubscription(this ILogger logger, SubscriptionId subscriptionId)
            => _handlingEventForSubscription(logger, subscriptionId.ConsumerTenantId, subscriptionId.PartitionId, subscriptionId.StreamId, subscriptionId.ProducerTenantId, subscriptionId.ProducerMicroserviceId, subscriptionId.ScopeId, null);

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Guid, Exception> _errorWhileHandlingEventFromSubscription = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Warning,
                new EventId(829474082, nameof(ErrorWhileHandlingEventFromSubscription)),
                "An error occurred while handling event horizon event for subscription: tenant {ConsumerTenant} from partition {Partition} in stream {Stream} from tenant {ProducerTenant} in microservice {ProducerMicroservice} into scope {Scope}");
        internal static void ErrorWhileHandlingEventFromSubscription(this ILogger logger, SubscriptionId subscriptionId, Exception exception)
            => _errorWhileHandlingEventFromSubscription(logger, subscriptionId.ConsumerTenantId, subscriptionId.PartitionId, subscriptionId.StreamId, subscriptionId.ProducerTenantId, subscriptionId.ProducerMicroserviceId, subscriptionId.ScopeId, exception);

#endregion


        static readonly Action<ILogger, Guid, Guid, Guid, string, int, Exception> _tenantSubscribedTo = LoggerMessage
            .Define<Guid, Guid, Guid, string, int>(
                LogLevel.Debug,
                new EventId(35626265, nameof(TenantSubscribedTo)),
                "Tenant: {ConsumerTenantId} is subscribing to events from tenant: {ProducerTenantId} in microservice: {ProducerMicroserviceId} on {Host}:{Port}");

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

        static readonly Action<ILogger, SubscriptionId, Exception> _retryProcessEvent = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(315586919, nameof(RetryProcessEvent)),
                "Retrying processing of event from Event Horizon for subscription: {Subscription}");

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Exception> _processEvent = LoggerMessage
            .Define<Guid, Guid, Guid, Guid>(
                LogLevel.Trace,
                new EventId(1329962982, nameof(ProcessEvent)),
                "Processing event type: {EventType} from event horizon in scope: {Scope} from microservice: {ProducerMicroservice} and tenant: {ProducerTenant}");

        static readonly Action<ILogger, SubscriptionId, Exception> _failedStartingSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(227530761, nameof(FailedStartingSubscription)),
                "Subscription: {SubscriptionId} failed");

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

        static readonly Action<ILogger, SubscriptionId, Exception> _subscriptionIsAlreadyRegistering = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Information,
                new EventId(2047860053, nameof(SubscriptionIsAlreadyRegistering)),
                "Subscription {Subscription} is already being registered");

        internal static void TenantSubscribedTo(this ILogger logger, TenantId consumerTenant, TenantId producerTenant, Microservice producerMicroservice, MicroserviceAddress microserviceAddress)
            => _tenantSubscribedTo(logger, consumerTenant, producerTenant, producerMicroservice, microserviceAddress.Host, microserviceAddress.Port, null);

        internal static void ReconnectingEventHorizon(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _reconnectingEventHorizon(logger, subscription, exception);

        internal static void ConnectedEventHorizon(this ILogger logger, SubscriptionId subscription)
            => _connectedEventHorizon(logger, subscription, null);

        internal static void ErrorInitializingSubscription(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _errorInitializingSubscription(logger, subscription, exception);

        internal static void ErrorWhileProcessingSubscription(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _errorWhileProcessingSubscription(logger, subscription, exception);

        internal static void RetryProcessEvent(this ILogger logger, SubscriptionId subscription)
            => _retryProcessEvent(logger, subscription, null);

        internal static void ProcessEvent(this ILogger logger, ArtifactId eventTypeId, ScopeId scope, Microservice producerMicroservice, TenantId producerTenant)
            => _processEvent(logger, eventTypeId, scope, producerMicroservice, producerTenant, null);

        internal static void FailedStartingSubscription(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _failedStartingSubscription(logger, subscription, exception);

        internal static void SuccessfullyRegisteredSubscription(this ILogger logger, SubscriptionId subscription)
            => _successfullyRegisteredSubscription(logger, subscription, null);

        internal static void UnregisteringSubscription(this ILogger logger, SubscriptionId subscription)
            => _unregisteringSubscription(logger, subscription, null);

        internal static void IncomingSubscripton(this ILogger logger, SubscriptionId subscription)
            => _incomingSubscripton(logger, subscription, null);

        internal static void ErrorWhileSubscribing(this ILogger logger, Exception exception, SubscriptionId subscription)
            => _errorWhileSubscribing(logger, subscription, exception);

        internal static void SubscriptionIsAlreadyRegistering(this ILogger logger, SubscriptionId subscription)
            => _subscriptionIsAlreadyRegistering(logger, subscription, null);
    }
}
