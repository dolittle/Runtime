// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    static class LoggerExtensions
    {

        #region StreamProcessor

        static readonly Action<ILogger, SubscriptionId, Exception> _streamProcessorCancellationRequested = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(233017281, nameof(StreamProcessorCancellationRequested)),
                "Stream Processor not starting because cancellation was requested for subscription {Subscription}");
        internal static void StreamProcessorCancellationRequested(this ILogger logger, SubscriptionId subscriptionId)
            => _streamProcessorCancellationRequested(logger, subscriptionId, null);

        static readonly Action<ILogger, SubscriptionId, Exception> _streamProcessorAlreadyProcessingStream = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Warning,
                new EventId(859878170, nameof(StreamProcessorAlreadyProcessingStream)),
                "Stream Processor is already processing stream for subscription {Subscription}");
        internal static void StreamProcessorAlreadyProcessingStream(this ILogger logger, SubscriptionId subscriptionId)
            => _streamProcessorAlreadyProcessingStream(logger, subscriptionId, null);

        static readonly Action<ILogger, SubscriptionId, Exception> _streamProcessorPersitingNewState = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Debug,
                new EventId(545783181, nameof(StreamProcessorPersitingNewState)),
                "Stream Processor persting new state for subscription {Subscription}");
        internal static void StreamProcessorPersitingNewState(this ILogger logger, SubscriptionId subscriptionId)
            => _streamProcessorPersitingNewState(logger, subscriptionId, null);

        static readonly Action<ILogger, StreamPosition, SubscriptionId, Exception> _streamProcessorFetchedState = LoggerMessage
            .Define<StreamPosition, SubscriptionId>(
                LogLevel.Debug,
                new EventId(532585404, nameof(StreamProcessorFetchedState)),
                "Stream Processor fetched state with next public event to receive {StreamPosition} for subscription {Subscription}");
        internal static void StreamProcessorFetchedState(this ILogger logger, SubscriptionId subscriptionId, IStreamProcessorState fetchedState)
            => _streamProcessorFetchedState(logger, fetchedState.Position, subscriptionId, null);

        #endregion

        #region EventProcessor

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Exception> _processEvent = LoggerMessage
            .Define<Guid, Guid, Guid, Guid>(
                LogLevel.Trace,
                new EventId(1329962982, nameof(ProcessEvent)),
                "Processing event type: {EventType} from event horizon in scope: {Scope} from microservice: {ProducerMicroservice} and tenant: {ProducerTenant}");
        internal static void ProcessEvent(this ILogger logger, ArtifactId eventTypeId, ScopeId scope, Microservice producerMicroservice, TenantId producerTenant)
            => _processEvent(logger, eventTypeId, scope, producerMicroservice, producerTenant, null);

        static readonly Action<ILogger, SubscriptionId, Exception> _retryProcessEvent = LoggerMessage
            .Define<SubscriptionId>(
                LogLevel.Trace,
                new EventId(315586919, nameof(RetryProcessEvent)),
                "Retry processing event from event horizon for subscription {Subscription}");
        internal static void RetryProcessEvent(this ILogger logger, SubscriptionId subscriptionId)
            => _retryProcessEvent(logger, subscriptionId, null);
        #endregion

    }
}
