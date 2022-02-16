// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.EventHorizon.Contracts.Subscriptions;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents the implementation of <see creF="FiltersBase"/>.
/// </summary>
[Singleton]
public class SubscriptionsService : SubscriptionsBase
{
    readonly Func<ISubscriptions> _getSubscriptions;
    readonly IExecutionContextManager _executionContextManager;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionsService"/> class.
    /// </summary>
    /// <param name="getSubscriptions">The <see cref="Func{T}" /> <see cref="ISubscriptions" />.</param>
    /// <param name="metrics">The system for capturing metrics.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public SubscriptionsService(
        Func<ISubscriptions> getSubscriptions,
        IExecutionContextManager executionContextManager,
        IMetricsCollector metrics,
        ILogger logger)
    {
        _executionContextManager = executionContextManager;
        _metrics = metrics;
        _getSubscriptions = getSubscriptions;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task<Contracts.SubscriptionResponse> Subscribe(Contracts.Subscription subscriptionRequest, ServerCallContext context)
    {
        _executionContextManager.CurrentFor(subscriptionRequest.CallContext.ExecutionContext);
        var subscription = new SubscriptionId(
            subscriptionRequest.CallContext.ExecutionContext.TenantId.ToGuid(),
            subscriptionRequest.MicroserviceId.ToGuid(),
            subscriptionRequest.TenantId.ToGuid(),
            subscriptionRequest.ScopeId.ToGuid(),
            subscriptionRequest.StreamId.ToGuid(),
            subscriptionRequest.PartitionId);
        try
        {
            _metrics.IncrementTotalSubscriptionsInitiatedFromHead();
            _logger.IncomingSubscripton(subscription);

            var subscriptionResponse = await _getSubscriptions().Subscribe(subscription).ConfigureAwait(false);

            return subscriptionResponse switch
            {
                { Success: false } => new Contracts.SubscriptionResponse { Failure = subscriptionResponse.Failure },
                _ => new Contracts.SubscriptionResponse { ConsentId = subscriptionResponse.ConsentId.ToProtobuf() },
            };
        }
        catch (TaskCanceledException)
        {
            return new Contracts.SubscriptionResponse { Failure = new Failure(SubscriptionFailures.SubscriptionCancelled, "Event Horizon subscription was cancelled") };
        }
        catch (Exception exception)
        {
            if (!context.CancellationToken.IsCancellationRequested)
            {
                _logger.ErrorWhileSubscribing(subscription, exception);
            }

            return new Contracts.SubscriptionResponse { Failure = new Failure(FailureId.Other, "InternalServerError") };
        }
    }
}
