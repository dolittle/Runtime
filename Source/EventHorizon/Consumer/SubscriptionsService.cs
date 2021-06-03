// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.EventHorizon.Contracts.Subscriptions;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    [Singleton]
    public class SubscriptionsService : SubscriptionsBase
    {
        readonly IConsumerClient _consumerClient;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsService"/> class.
        /// </summary>
        /// <param name="consumerClient">The <see cref="IConsumerClient" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public SubscriptionsService(IConsumerClient consumerClient, ILogger logger)
        {
            _consumerClient = consumerClient;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<Contracts.SubscriptionResponse> Subscribe(Contracts.Subscription subscriptionRequest, ServerCallContext context)
        {
            var subscription = new SubscriptionId(
                subscriptionRequest.CallContext.ExecutionContext.TenantId.ToGuid(),
                subscriptionRequest.MicroserviceId.ToGuid(),
                subscriptionRequest.TenantId.ToGuid(),
                subscriptionRequest.ScopeId.ToGuid(),
                subscriptionRequest.StreamId.ToGuid(),
                subscriptionRequest.PartitionId.ToGuid());
            try
            {
                _logger.IncomingSubscripton(subscription);
                var subscriptionResponse = await _consumerClient.HandleSubscriptionRequest(subscription, context.CancellationToken).ConfigureAwait(false);

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
            catch (Exception ex)
            {
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    _logger.ErrorWhileSubscribing(ex, subscription);
                }

                return new Contracts.SubscriptionResponse { Failure = new Failure(FailureId.Other, "InternalServerError") };
            }
        }
    }
}
