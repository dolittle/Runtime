// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections
{
    static class LoggerExtensions
    {

        #region EventHorizonConnection

        static readonly Action<ILogger, SubscriptionId, Exception> _connectionToProducerRuntimeSucceeded = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(1573331110, nameof(ConnectionToProducerRuntimeSucceeded)),
                "Connection to producer runtime succeeded for subscription {Subscription}");
        internal static void ConnectionToProducerRuntimeSucceeded(this ILogger logger, SubscriptionId subscriptionId)
            => _connectionToProducerRuntimeSucceeded(logger, subscriptionId, null);

        static readonly Action<ILogger, string, SubscriptionId, Exception> _connectionToProducerRuntimeFailed = LoggerMessage
            .Define<string, SubscriptionId>(
                LogLevel.Warning,
                new EventId(265824309, nameof(ConnectionToProducerRuntimeFailed)),
                "Connection to producer Runtime failed because {Reason} for subscription {Subscription}");
        internal static void ConnectionToProducerRuntimeFailed(this ILogger logger, SubscriptionId subscriptionId, FailureReason reason)
            => _connectionToProducerRuntimeFailed(logger, reason, subscriptionId, null);

        static readonly Action<ILogger, SubscriptionId, Exception> _couldNotConnectToProducerRuntime = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(305416696, nameof(CouldNotConnectToProducerRuntime)),
                "Could not connect to producer Runtime for subscription {Subscription}");
        internal static void CouldNotConnectToProducerRuntime(this ILogger logger, SubscriptionId subscriptionId)
            => _couldNotConnectToProducerRuntime(logger, subscriptionId, null);

        static readonly Action<ILogger, SubscriptionId, Exception> _handlingEventForSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(844153427, nameof(HandlingEventForSubscription)),
                "Handling event horizon event for subscription {Subscription}");
        internal static void HandlingEventForSubscription(this ILogger logger, SubscriptionId subscriptionId)
            => _handlingEventForSubscription(logger, subscriptionId, null);

        static readonly Action<ILogger, SubscriptionId, Exception> _errorWhileHandlingEventFromSubscription = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(829474082, nameof(ErrorWhileHandlingEventFromSubscription)),
                "An error occurred while handling event horizon event for subscription {Subscription}");
        internal static void ErrorWhileHandlingEventFromSubscription(this ILogger logger, SubscriptionId subscriptionId, Exception exception)
            => _errorWhileHandlingEventFromSubscription(logger, subscriptionId, exception);

        #endregion
    }
}
