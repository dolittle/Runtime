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

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Represent an implementation <see cref="IEventHorizonProcessor" />. 
    /// </summary>
    public class EventHorizonProcessor : IEventHorizonProcessor
    {
        readonly IReverseCallClient<ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> _reverseCallClient;
        readonly SubscriptionId _subscription;
        readonly AsyncProducerConsumerQueue<StreamEvent> _eventsFromEventHorizon;
        readonly ILogger _logger;
        readonly CancellationToken _cancellationToken;

        public EventHorizonProcessor(
            IReverseCallClient<ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> reverseCallClient,
            SubscriptionId subscription,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            _reverseCallClient = reverseCallClient;
            _subscription = subscription;
            _eventsFromEventHorizon = eventsFromEventHorizon;
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        /// <inheritdoc/>
        public async Task<SubscriptionResponse> Connect(StreamPosition publicEventsPosition)
        {
            var connected = await _reverseCallClient.Connect(CreateRequest(publicEventsPosition), _cancellationToken).ConfigureAwait(false);
            var response = _reverseCallClient.ConnectResponse;
            if (!connected || response == null)
            {
                _logger.DidNotReceiveSubscriptionResponse(_subscription);
                return SubscriptionResponse.Failed(new Failure(SubscriptionFailures.DidNotReceiveSubscriptionResponse, "Did not receive a subscription response when subscribing"));
            }

            var failure = response.Failure;
            if (failure != null)
            {
                _logger.FailedSubscring(_subscription, failure.Reason);
                return SubscriptionResponse.Failed(failure);
            }

            _logger.SuccessfulSubscring(_subscription);
            return SubscriptionResponse.Succeeded(response.ConsentId.ToGuid());
        }

        /// <inheritdoc/>
        public Task StartHandleEvents()
            => _reverseCallClient.Handle(
                (request, _) => HandleEventFromEventHorizon(request.Event),
                _cancellationToken);

        async Task<ConsumerResponse> HandleEventFromEventHorizon(EventHorizonEvent @event)
        {
            try
            {
                await _eventsFromEventHorizon.EnqueueAsync(
                    new StreamEvent(
                        @event.Event.ToCommittedEvent(),
                        @event.StreamSequenceNumber,
                        StreamId.EventLog,
                        Guid.Empty,
                        false),
                    _cancellationToken).ConfigureAwait(false);
                return CreateSuccessfulResponse();
            }
            catch (Exception ex)
            {
                _logger.ErrorWhileHandlingEventFromSubscription(ex, _subscription);
                return CreateFailureResponse(new Failure(
                    FailureId.Other,
                    $"An error occurred while handling event horizon event coming from subscription {_subscription}. {ex.Message}"));
            }
        }

        ConsumerSubscriptionRequest CreateRequest(StreamPosition publicEventsPosition)
            => new()
            {
                PartitionId = _subscription.PartitionId.ToProtobuf(),
                StreamId = _subscription.StreamId.ToProtobuf(),
                StreamPosition = publicEventsPosition.Value,
                TenantId = _subscription.ProducerTenantId.ToProtobuf()
            };

        ConsumerResponse CreateSuccessfulResponse() => new();
        ConsumerResponse CreateFailureResponse(Failure failure) => new() { Failure = failure };
    }

}
