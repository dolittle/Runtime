// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Services.Clients;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizonConnection" />.
    /// </summary>
    public class EventHorizonConnection : IEventHorizonConnection
    {
        readonly SubscriptionId _subscription;
        readonly MicroserviceAddress _connectionAddress;
        readonly Contracts.Consumer.ConsumerClient _client;
        readonly IAsyncPolicyFor<ConsumerClient> _policy;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IReverseCallClients _reverseCallClients;
        readonly AsyncProducerConsumerQueue<StreamEvent> _eventsFromEventHorizon;
        readonly ILogger _logger;
        readonly CancellationToken _cancellationToken;

        public EventHorizonConnection(
            SubscriptionId subscription,
            MicroserviceAddress connectionAddress,
            Contracts.Consumer.ConsumerClient client,
            IAsyncPolicyFor<ConsumerClient> policy,
            IStreamProcessorStateRepository streamProcessorStates,
            IReverseCallClients reverseCallClients,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            _subscription = subscription;
            _connectionAddress = connectionAddress;
            _client = client;
            _policy = policy;
            _streamProcessorStates = streamProcessorStates;
            _reverseCallClients = reverseCallClients;
            _eventsFromEventHorizon = eventsFromEventHorizon;
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        /// <inheritdoc/>
        public Task<SubscriptionResponse> InitiateAndKeepConnection()
            => _policy.Execute(
                async _ =>
                {
                    var (response, _) = await ConnectAndDo(
                        reverseCallClient => Task.Run(() => StartProcessingAndKeepAlive(reverseCallClient)),
                        () => { }).ConfigureAwait(false);

                    return response;
                },
                _cancellationToken);

        async Task<SubscriptionResponse> Connect(
            IReverseCallClient<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> reverseCallClient)
        {
            _logger.TenantSubscribedTo(
                _subscription.ConsumerTenantId,
                _subscription.ProducerTenantId,
                _subscription.ProducerMicroserviceId,
                _connectionAddress);
            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(_subscription, _cancellationToken).ConfigureAwait(false);

            var publicEventsPosition = tryGetStreamProcessorState.Result?.Position ?? StreamPosition.Start;
            var receivedResponse = await reverseCallClient.Connect(
                new ConsumerSubscriptionRequest
                {
                    PartitionId = _subscription.PartitionId.ToProtobuf(),
                    StreamId = _subscription.StreamId.ToProtobuf(),
                    StreamPosition = publicEventsPosition.Value,
                    TenantId = _subscription.ProducerTenantId.ToProtobuf()
                },
                _cancellationToken).ConfigureAwait(false);

            if (!receivedResponse)
            {
                _logger.DidNotReceiveSubscriptionResponse(_subscription);
                return SubscriptionResponse.Failed(new Failure(SubscriptionFailures.DidNotReceiveSubscriptionResponse, "Did not receive a subscription response when subscribing"));
            }

            var failure = reverseCallClient.ConnectResponse.Failure;
            if (failure != null)
            {
                _logger.FailedSubscring(_subscription, failure.Reason);
                return SubscriptionResponse.Failed(failure);
            }
            _logger.SuccessfulSubscring(_subscription);
            return SubscriptionResponse.Succeeded(reverseCallClient.ConnectResponse.ConsentId.ToGuid());
        }

        async Task StartProcessingAndKeepAlive(
            IReverseCallClient<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> reverseCallClient
        )
        {
            try
            {
                await ReadEventsFromEventHorizon(reverseCallClient).ConfigureAwait(false);
                throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values
            }
            catch (Exception ex)
            {
                _logger.ReconnectingEventHorizon(ex, _subscription);
                await _policy.Execute(
                    async _ =>
                    {
                        var (response, newReverseCallClient) = await ConnectAndDo(_ => { }, () => throw new Todo());
                        reverseCallClient = newReverseCallClient;
                        await ReadEventsFromEventHorizon(reverseCallClient).ConfigureAwait(false);
                        throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values
                    }, _cancellationToken).ConfigureAwait(false);
            }
        }
        async Task ReadEventsFromEventHorizon(
            IReverseCallClient<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> reverseCallClient)
        {
            _logger.ConnectedEventHorizon(_subscription);

            await reverseCallClient.Handle(
                (request, cancellationToken) => HandleConsumerRequest(request.Event, cancellationToken),
                _cancellationToken).ConfigureAwait(false);
        }

        async Task<ConsumerResponse> HandleConsumerRequest(
            EventHorizonEvent @event,
            CancellationToken cancellationToken)
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
                    cancellationToken).ConfigureAwait(false);
                return new ConsumerResponse();
            }
            catch (Exception ex)
            {
                _logger.ErrorWhileHandlingEventFromSubscription(ex, _subscription);
                return new ConsumerResponse
                {
                    Failure = new Failure(FailureId.Other, $"An error occurred while handling event horizon event coming from subscription {_subscription}. {ex.Message}")
                };
            }
        }

        async Task<(SubscriptionResponse, IReverseCallClient<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse>)> ConnectAndDo(
            Action<IReverseCallClient<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse>> onConnected,
            Action onNotConnected
        )
        {
            var reverseCallClient = CreateClient();
            var response = await Connect(reverseCallClient).ConfigureAwait(false);
            if (response.Success)
            {
                onConnected(reverseCallClient);
            }
            else
            {
                onNotConnected();
            }
            return (response, reverseCallClient);
        }

        IReverseCallClient<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> CreateClient()
              => _reverseCallClients.GetFor<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse>(
                  () => _client.Subscribe(cancellationToken: _cancellationToken),
                  (message, arguments) => message.SubscriptionRequest = arguments,
                  message => message.SubscriptionResponse,
                  message => message.Request,
                  (message, response) => message.Response = response,
                  (arguments, context) => arguments.CallContext = context,
                  request => request.CallContext,
                  (response, context) => response.CallContext = context,
                  message => message.Ping,
                  (message, pong) => message.Pong = pong,
                  TimeSpan.FromSeconds(7));
    }
}
