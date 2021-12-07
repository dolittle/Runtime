// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Event Store is unavailable")]
    internal static partial void EventStoreIsUnavailable(ILogger logger, EventStoreUnavailable ex);

    [LoggerMessage(0, LogLevel.Warning, "Stream Processor not starting because cancellation was requested for subscription {SubscriptionId}")]
    internal static partial void StreamProcessorCancellationRequested(ILogger logger, SubscriptionId subscriptionId);

    [LoggerMessage(0, LogLevel.Warning, "Stream Processor is already processing stream for subscription {SubscriptionId}")]
    internal static partial void StreamProcessorAlreadyProcessingStream(ILogger logger, SubscriptionId subscriptionId);

    [LoggerMessage(0, LogLevel.Debug, "Stream Processor persisting new state for subscription {SubscriptionId}")]
    internal static partial void StreamProcessorPersistingNewState(ILogger logger, SubscriptionId subscriptionId);
    
    [LoggerMessage(0, LogLevel.Debug, "Stream Processor fetched state with next public event to receive {NextPublicEventPosition} for subscription {SubscriptionId}")]
    internal static partial void StreamProcessorFetchedState(ILogger logger, SubscriptionId subscriptionId, StreamPosition nextPublicEventPosition);


    [LoggerMessage(0, LogLevel.Trace, "Processing event type: {EventTypeId} from event horizon in scope: {Scope} from microservice: {ProducerMicroservice} and tenant: {ProducerTenant}")]
    internal static partial void ProcessEvent(ILogger logger, ArtifactId eventTypeId, ScopeId scope, MicroserviceId producerMicroservice, TenantId producerTenant);

    [LoggerMessage(0, LogLevel.Trace, "Retry processing event from event horizon for subscription {SubscriptionId}")]
    internal static partial void RetryProcessEvent(ILogger logger, SubscriptionId subscriptionId);

}
