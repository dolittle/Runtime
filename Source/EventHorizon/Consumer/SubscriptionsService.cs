// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.EventHorizon.Contracts.Subscriptions;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services.Hosting;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents the implementation of <see creF="FiltersBase"/>.
/// </summary>
[PrivateService]
public class SubscriptionsService : SubscriptionsBase
{
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly Func<TenantId, ISubscriptions> _getSubscriptionsFor;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionsService"/> class.
    /// </summary>
    /// <param name="executionContextCreator">The execution context creator to use for verifying incoming execution contexts.</param>
    /// <param name="getSubscriptionsFor">The factory to use to create an <see cref="ISubscriptions"/> for a tenant.</param>
    /// <param name="metrics">The system for capturing metrics.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public SubscriptionsService(
        ICreateExecutionContexts executionContextCreator,
        Func<TenantId, ISubscriptions> getSubscriptionsFor,
        IMetricsCollector metrics,
        ILogger logger)
    {
        _executionContextCreator = executionContextCreator;
        _getSubscriptionsFor = getSubscriptionsFor;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task<Contracts.SubscriptionResponse> Subscribe(Contracts.Subscription subscriptionRequest, ServerCallContext context)
    {
        var createExecutionContext = _executionContextCreator.TryCreateUsing(subscriptionRequest.CallContext.ExecutionContext);
        if (!createExecutionContext.Success)
        {
            return new Contracts.SubscriptionResponse
            {
                Failure = new Dolittle.Protobuf.Contracts.Failure
                {
                    Id = FailureId.Other.ToProtobuf(),
                    Reason = $"Failed to create event horizon subscription because execution context was invalid: {createExecutionContext.Exception.Message}",
                }
            };
        }

        var executionContext = createExecutionContext.Result;
        
        var subscription = new SubscriptionId(
            executionContext.Tenant,
            subscriptionRequest.MicroserviceId.ToGuid(),
            subscriptionRequest.TenantId.ToGuid(),
            subscriptionRequest.ScopeId.ToGuid(),
            subscriptionRequest.StreamId.ToGuid(),
            subscriptionRequest.PartitionId);
        
        try
        {
            _metrics.IncrementTotalSubscriptionsInitiatedFromHead();
            _logger.IncomingSubscripton(subscription);

            var subscriptionResponse = await _getSubscriptionsFor(executionContext.Tenant).Subscribe(subscription, executionContext).ConfigureAwait(false);

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
