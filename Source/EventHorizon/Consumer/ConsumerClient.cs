// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Resilience;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Services.Clients;
using Grpc.Core;
using Nito.AsyncEx;
using grpc = contracts::Dolittle.Runtime.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IConsumerClient" />.
    /// </summary>
    public class ConsumerClient : IConsumerClient
    {
        readonly IClientManager _clientManager;
        readonly ISubscriptions _subscriptions;
        readonly MicroservicesConfiguration _microservicesConfiguration;
        readonly IStreamProcessors _streamProcessors;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IWriteEventHorizonEvents _eventHorizonEventsWriter;
        readonly IAsyncPolicyFor<ConsumerClient> _policy;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerClient"/> class.
        /// </summary>
        /// <param name="clientManager">The <see cref="IClientManager" />.</param>
        /// <param name="subscriptions">The <see cref="ISubscriptions" />.</param>
        /// <param name="microservicesConfiguration">The <see cref="MicroservicesConfiguration" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="eventHorizonEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ConsumerClient" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ConsumerClient(
            IClientManager clientManager,
            ISubscriptions subscriptions,
            MicroservicesConfiguration microservicesConfiguration,
            IStreamProcessors streamProcessors,
            IStreamProcessorStateRepository streamProcessorStates,
            IWriteEventHorizonEvents eventHorizonEventsWriter,
            IAsyncPolicyFor<ConsumerClient> policy,
            ILogger logger)
        {
            _clientManager = clientManager;
            _subscriptions = subscriptions;
            _microservicesConfiguration = microservicesConfiguration;
            _streamProcessors = streamProcessors;
            _streamProcessorStates = streamProcessorStates;
            _eventHorizonEventsWriter = eventHorizonEventsWriter;
            _policy = policy;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<SubscriptionResponse> HandleSubscription(Subscription subscription)
        {
            if (!TryAddNewSubscription(subscription))
            {
                _logger.Trace($"Already subscribed to subscription {subscription}");
                return new SuccessfulSubscriptionResponse();
            }

            if (!TryGetMicroserviceAddress(subscription.ProducerMicroservice, out var microserviceAddress))
            {
                var message = $"There is no microservice configuration for the producer microservice '{subscription.ProducerMicroservice}'.";
                _logger.Warning(message);
                return new FailedSubscriptionResponse(message);
            }

            var call = await Subscribe(subscription, microserviceAddress).ConfigureAwait(false);
            var response = await HandleSubscriptionResponse(
                call.ResponseStream,
                subscription).ConfigureAwait(false);

            if (response.Success) StartProcessingEventHorizon(subscription, microserviceAddress, call.ResponseStream);
            return response;
        }

        async Task<AsyncServerStreamingCall<grpc.SubscriptionStreamMessage>> Subscribe(Subscription subscription, MicroserviceAddress microserviceAddress)
        {
            _logger.Trace($"Tenant '{subscription.ConsumerTenant}' is subscribing to events from tenant '{subscription.ProducerTenant}' in microservice '{subscription.ProducerMicroservice}' on address '{microserviceAddress.Host}:{microserviceAddress.Port}'");
            var currentStreamProcessorState = await _streamProcessorStates.GetOrAddNew(new StreamProcessorId(
                                                                                        subscription.Scope,
                                                                                        subscription.ProducerTenant.Value,
                                                                                        subscription.ProducerMicroservice.Value)).ConfigureAwait(false);
            var publicEventsPosition = currentStreamProcessorState.Position;
            return _clientManager
                .Get<grpc.Consumer.ConsumerClient>(microserviceAddress.Host, microserviceAddress.Port)
                .Subscribe(
                    new grpc.ConsumerSubscription
                    {
                        Tenant = subscription.ProducerTenant.ToProtobuf(),
                        LastReceived = (int)publicEventsPosition.Value - 1,
                        Stream = subscription.Stream.ToProtobuf(),
                        Partition = subscription.Partition.ToProtobuf()
                    });
        }

        async Task<SubscriptionResponse> HandleSubscriptionResponse(IAsyncStreamReader<grpc.SubscriptionStreamMessage> responseStream, Subscription subscription)
        {
            if (!await responseStream.MoveNext(CancellationToken.None).ConfigureAwait(false)
                || responseStream.Current.MessageCase != grpc.SubscriptionStreamMessage.MessageOneofCase.SubscriptionResponse)
            {
                var message = $"Did not receive subscription response when subscribing with subscription {subscription}";
                _logger.Warning(message);
                return new FailedSubscriptionResponse(message);
            }

            var subscriptionResponse = responseStream.Current.SubscriptionResponse;
            if (subscriptionResponse.Failure != null)
            {
                return new FailedSubscriptionResponse(subscriptionResponse.Failure.Reason);
            }

            return new SuccessfulSubscriptionResponse();
        }

        void StartProcessingEventHorizon(Subscription subscription, MicroserviceAddress microserviceAddress, IAsyncStreamReader<grpc.SubscriptionStreamMessage> responseStream)
        {
            Task.Run(async () =>
                {
                    var cancellationToken = CancellationToken.None;
                    try
                    {
                        await ReadEventsFromEventHorizon(subscription, responseStream, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        _logger.Debug($"");
                        await _policy.Execute(
                            async () =>
                            {
                                var call = await Subscribe(subscription, microserviceAddress).ConfigureAwait(false);
                                var response = await HandleSubscriptionResponse(call.ResponseStream, subscription).ConfigureAwait(false);
                                if (!response.Success) throw new Todo();
                                await ReadEventsFromEventHorizon(subscription, call.ResponseStream, cancellationToken).ConfigureAwait(false);
                            }).ConfigureAwait(false);
                    }
                });
        }

        async Task ReadEventsFromEventHorizon(Subscription subscription, IAsyncStreamReader<grpc.SubscriptionStreamMessage> responseStream, CancellationToken cancellationToken)
        {
            var queue = new AsyncProducerConsumerQueue<StreamEvent>();
            var eventsFetcher = new EventsFromEventHorizonFetcher(queue);
            _streamProcessors.Register(
                new EventProcessor(subscription, _eventHorizonEventsWriter, _logger),
                eventsFetcher,
                subscription.ProducerMicroservice.Value,
                cancellationToken);

            while (await responseStream.MoveNext(cancellationToken).ConfigureAwait(false))
            {
                if (responseStream.Current.MessageCase != grpc.SubscriptionStreamMessage.MessageOneofCase.Event)
                {
                    _logger.Warning($"");
                    continue;
                }

                var @event = responseStream.Current.Event;
                await queue.EnqueueAsync(new StreamEvent(
                    @event.ToCommittedEvent(subscription.ProducerMicroservice, subscription.ConsumerTenant),
                    @event.StreamSequenceNumber,
                    StreamId.AllStreamId,
                    PartitionId.NotSet)).ConfigureAwait(false);
            }
        }

        bool TryAddNewSubscription(Subscription subscription) =>
            _subscriptions.AddSubscription(subscription);

        bool TryGetMicroserviceAddress(Microservice producerMicroservice, out MicroserviceAddress microserviceAddress) =>
            _microservicesConfiguration.TryGetValue(producerMicroservice, out microserviceAddress);
    }
}