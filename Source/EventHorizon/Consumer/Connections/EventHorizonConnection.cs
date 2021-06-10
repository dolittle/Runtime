// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Clients;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections
{
    /// <summary>
    /// Represent an implementation <see cref="IEventHorizonConnection" />. 
    /// </summary>
    public class EventHorizonConnection : IEventHorizonConnection
    {
        readonly IReverseCallClient<ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> _reverseCallClient;
        readonly IMetricsCollector _metrics;
        readonly ILogger _logger;
        SubscriptionId _subscriptionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonConnection" /> class.
        /// </summary>
        /// <param name="reverseCallClient">The reverse call client.</param>
        /// <param name="metrics">The system for capturing metrics.</param>
        /// <param name="logger">The logger.</param>
        public EventHorizonConnection(
            IReverseCallClient<ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> reverseCallClient,
            IMetricsCollector metrics,
            ILogger logger)
        {
            _reverseCallClient = reverseCallClient;
            _metrics = metrics;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<SubscriptionResponse> Connect(
            SubscriptionId subscription,
            StreamPosition publicEventsPosition,
            CancellationToken cancellationToken)
        {
            if (await _reverseCallClient.Connect(CreateRequest(subscription, publicEventsPosition), cancellationToken).ConfigureAwait(false))
            {
                _metrics.IncrementTotalConnectionAttempts();
                _subscriptionId = subscription;

                var response = _reverseCallClient.ConnectResponse;

                if (response.Failure != default)
                {
                    _metrics.IncrementTotalFailureResponses();
                    _logger.ConnectionToProducerRuntimeFailed(subscription, response.Failure.Reason);
                    return SubscriptionResponse.Failed(response.Failure);
                }
                _metrics.IncrementTotalSuccessfulResponses();
                _logger.ConnectionToProducerRuntimeSucceeded(subscription);
                return SubscriptionResponse.Succeeded(response.ConsentId.ToGuid());
            }

            _metrics.IncrementTotalConnectionsFailed();
            _logger.CouldNotConnectToProducerRuntime(subscription);
            return SubscriptionResponse.Failed(
                new Failure(
                    SubscriptionFailures.CouldNotConnectToProducerRuntime,
                    "Could not connect to producer Runtime"));
        }

        /// <inheritdoc/>
        public Task StartReceivingEventsInto(
            AsyncProducerConsumerQueue<StreamEvent> connectionToStreamProcessorQueue,
            CancellationToken cancellationToken)
        {
            return _reverseCallClient.Handle(
                (request, cancellationToken)
                    => HandleEventFromEventHorizon(
                        connectionToStreamProcessorQueue,
                        request.Event,
                        cancellationToken),
                cancellationToken);
        }

        async Task<ConsumerResponse> HandleEventFromEventHorizon(
            AsyncProducerConsumerQueue<StreamEvent> connectionToStreamProcessorQueue,
            EventHorizonEvent @event,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.HandlingEventForSubscription(_subscriptionId);
                var streamEvent = new StreamEvent(
                        @event.Event.ToCommittedEvent(),
                        @event.StreamSequenceNumber,
                        StreamId.EventLog,
                        Guid.Empty,
                        false);

                await connectionToStreamProcessorQueue.EnqueueAsync(streamEvent, cancellationToken).ConfigureAwait(false);
                return CreateSuccessfulResponse();
            }
            catch (Exception exception)
            {
                _logger.ErrorWhileHandlingEventFromSubscription(_subscriptionId, exception);
                return CreateFailureResponse(new Failure(
                    SubscriptionFailures.ErrorHandlingEvent,
                    exception.Message));
            }
        }

        ConsumerSubscriptionRequest CreateRequest(SubscriptionId subscription, StreamPosition publicEventsPosition)
            => new()
            {
                PartitionId = subscription.PartitionId.ToProtobuf(),
                StreamId = subscription.StreamId.ToProtobuf(),
                StreamPosition = publicEventsPosition.Value,
                TenantId = subscription.ProducerTenantId.ToProtobuf()
            };

        ConsumerResponse CreateSuccessfulResponse() => new();

        ConsumerResponse CreateFailureResponse(Failure failure) => new() { Failure = failure };
    }
}
