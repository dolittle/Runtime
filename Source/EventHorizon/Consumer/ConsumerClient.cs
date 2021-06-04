// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Services.Clients;
using Nito.AsyncEx;

using EventHorizonReverseCallClient = Dolittle.Runtime.Services.Clients.IReverseCallClient<
    Dolittle.Runtime.EventHorizon.Contracts.ConsumerSubscriptionRequest,
    Dolittle.Runtime.EventHorizon.Contracts.SubscriptionResponse,
    Dolittle.Runtime.EventHorizon.Contracts.ConsumerRequest,
    Dolittle.Runtime.EventHorizon.Contracts.ConsumerResponse>;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IConsumerClient" />.
    /// </summary>
    [SingletonPerTenant]
    public class ConsumerClient : IConsumerClient
    {
        readonly ISubscriptions _subscriptions;
        readonly MicroservicesConfiguration _microservicesConfiguration;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IWriteEventHorizonEvents _eventHorizonEventsWriter;
        readonly IAsyncPolicyFor<ConsumerClient> _policy;
        readonly IAsyncPolicyFor<EventProcessor> _eventProcessorPolicy;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly CancellationToken _cancellationToken;
        readonly IReverseCallClients _reverseCallClients;
        readonly ILogger _logger;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerClient"/> class.
        /// </summary>
        /// <param name="subscriptions">The <see cref="ISubscriptions" />.</param>
        /// <param name="microservicesConfiguration">The <see cref="MicroservicesConfiguration" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="eventHorizonEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ConsumerClient" />.</param>
        /// <param name="eventProcessorPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="EventProcessor" />.</param>
        /// <param name="reverseCallClients"><see cref="IReverseCallClients"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ConsumerClient(
            ISubscriptions subscriptions,
            MicroservicesConfiguration microservicesConfiguration,
            IStreamProcessorStateRepository streamProcessorStates,
            IWriteEventHorizonEvents eventHorizonEventsWriter,
            IAsyncPolicyFor<ConsumerClient> policy,
            IAsyncPolicyFor<EventProcessor> eventProcessorPolicy,
            IReverseCallClients reverseCallClients,
            ILogger logger)
        {
            _subscriptions = subscriptions;
            _microservicesConfiguration = microservicesConfiguration;
            _streamProcessorStates = streamProcessorStates;
            _eventHorizonEventsWriter = eventHorizonEventsWriter;
            _policy = policy;
            _eventProcessorPolicy = eventProcessorPolicy;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _reverseCallClients = reverseCallClients;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ConsumerClient"/> class.
        /// </summary>
        ~ConsumerClient()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task<SubscriptionResponse> HandleSubscription(SubscriptionId subscriptionId)
        {
            if (_subscriptions.TryGetConsentFor(subscriptionId, out var consentId))
            {
                _logger.SubscriptionAlreadyRegistered(subscriptionId);
                return new SuccessfulSubscriptionResponse(consentId);
            }

            if (!TryGetMicroserviceAddress(subscriptionId.ProducerMicroserviceId, out var microserviceAddress))
            {
                _logger.NoMicroserviceConfigurationFor(subscriptionId.ProducerMicroserviceId);
                return new FailedSubscriptionResponse(new Failure(
                    SubscriptionFailures.MissingMicroserviceConfiguration,
                    $"No microservice configuration for producer mircoservice {subscriptionId.ProducerMicroserviceId}"));
            }

            return await _policy.Execute(
                async _ =>
                {
                    var client = CreateClient(microserviceAddress);
                    var receivedResponse = await Subscribe(client, subscriptionId, microserviceAddress, _cancellationToken).ConfigureAwait(false);
                    var response = HandleSubscriptionResponse(receivedResponse, client.ConnectResponse, subscriptionId);
                    if (response.Success) StartProcessingEventHorizon(response.ConsentId, subscriptionId, microserviceAddress, client);
                    return response;
                }, _cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposeManagedResources">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_disposed) return;

            _cancellationTokenSource.Cancel();
            if (disposeManagedResources)
            {
                _cancellationTokenSource.Dispose();
            }

            _disposed = true;
        }

        EventHorizonReverseCallClient CreateClient(MicroserviceAddress microserviceAddress)
            => _reverseCallClients.GetFor(new EventHorizonProtocol(), microserviceAddress.Host, microserviceAddress.Port, TimeSpan.FromSeconds(7));

        async Task<bool> Subscribe(
            EventHorizonReverseCallClient reverseCallClient,
            SubscriptionId subscriptionId,
            MicroserviceAddress microserviceAddress,
            CancellationToken cancellationToken)
        {
            _logger.TenantSubscribedTo(
                subscriptionId.ConsumerTenantId,
                subscriptionId.ProducerTenantId,
                subscriptionId.ProducerMicroserviceId,
                microserviceAddress);
            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(subscriptionId, cancellationToken).ConfigureAwait(false);

            var publicEventsPosition = tryGetStreamProcessorState.Result?.Position ?? StreamPosition.Start;
            return await reverseCallClient.Connect(
                new ConsumerSubscriptionRequest
                {
                    PartitionId = subscriptionId.PartitionId.ToProtobuf(),
                    StreamId = subscriptionId.StreamId.ToProtobuf(),
                    StreamPosition = publicEventsPosition.Value,
                    TenantId = subscriptionId.ProducerTenantId.ToProtobuf()
                }, cancellationToken).ConfigureAwait(false);
        }

        SubscriptionResponse HandleSubscriptionResponse(bool receivedResponse, Contracts.SubscriptionResponse subscriptionResponse, SubscriptionId subscriptionId)
        {
            if (!receivedResponse)
            {
                _logger.DidNotReceiveSubscriptionResponse(subscriptionId);
                return new FailedSubscriptionResponse(new Failure(SubscriptionFailures.DidNotReceiveSubscriptionResponse, "Did not receive a subscription response when subscribing"));
            }

            if (subscriptionResponse.Failure != null)
            {
                _logger.FailedSubscring(subscriptionId, subscriptionResponse.Failure.Reason);
                return new FailedSubscriptionResponse(subscriptionResponse.Failure);
            }

            _logger.SuccessfulSubscring(subscriptionId);
            return new SuccessfulSubscriptionResponse(subscriptionResponse.ConsentId.ToGuid());
        }

        void StartProcessingEventHorizon(
            ConsentId consentId,
            SubscriptionId subscriptionId,
            MicroserviceAddress microserviceAddress,
            EventHorizonReverseCallClient reverseCallClient)
        {
            Task.Run(async () =>
                {
                    try
                    {
                        await ReadEventsFromEventHorizon(consentId, subscriptionId, reverseCallClient).ConfigureAwait(false);
                        throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values
                    }
                    catch (Exception ex)
                    {
                        _logger.ReconnectingEventHorizon(ex, subscriptionId);
                        await _policy.Execute(
                            async _ =>
                            {
                                reverseCallClient = CreateClient(microserviceAddress);
                                var receivedResponse = await Subscribe(reverseCallClient, subscriptionId, microserviceAddress, _cancellationToken).ConfigureAwait(false);
                                var response = HandleSubscriptionResponse(receivedResponse, reverseCallClient.ConnectResponse, subscriptionId);
                                if (!response.Success) throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values

                                await ReadEventsFromEventHorizon(response.ConsentId, subscriptionId, reverseCallClient).ConfigureAwait(false);
                                throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values
                            }, _cancellationToken).ConfigureAwait(false);
                    }
                });
        }

        async Task ReadEventsFromEventHorizon(
            ConsentId consentId,
            SubscriptionId subscriptionId,
            EventHorizonReverseCallClient reverseCallClient)
            {
            _logger.ConnectedEventHorizon(subscriptionId);
            var queue = new AsyncProducerConsumerQueue<StreamEvent>();
            var eventsFetcher = new EventsFromEventHorizonFetcher(queue);

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
            var tasks = new List<Task>();
            try
            {
                _subscriptions.TrySubscribe(
                    consentId,
                    subscriptionId,
                    new EventProcessor(consentId, subscriptionId, _eventHorizonEventsWriter, _eventProcessorPolicy, _logger),
                    eventsFetcher,
                    linkedTokenSource.Token,
                    out var outputtedStreamProcessor);
                using var streamProcessor = outputtedStreamProcessor;
                await streamProcessor.Initialize().ConfigureAwait(false);

                tasks.Add(Task.Run(async () =>
                    {
                        await reverseCallClient.Handle(
                            (request, cancellationToken) => HandleConsumerRequest(subscriptionId, queue, request, cancellationToken),
                            linkedTokenSource.Token).ConfigureAwait(false);
                        linkedTokenSource.Cancel();
                    }));
                tasks.Add(streamProcessor.Start());
            }
            catch (Exception ex)
            {
                linkedTokenSource.Cancel();
                _logger.ErrorInitializingSubscription(ex, subscriptionId);
                return;
            }

            var finishedTask = await Task.WhenAny(tasks).ConfigureAwait(false);
            if (!linkedTokenSource.IsCancellationRequested) linkedTokenSource.Cancel();
            if (TryGetException(tasks, out var exception))
            {
                linkedTokenSource.Cancel();
                _logger.ErrorWhileProcessingSubscription(exception, subscriptionId);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        async Task<ConsumerResponse> HandleConsumerRequest(
            SubscriptionId subscriptionId,
            AsyncProducerConsumerQueue<StreamEvent> queue,
            ConsumerRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var eventHorizonEvent = request.Event;
                await queue.EnqueueAsync(
                    new StreamEvent(
                        eventHorizonEvent.Event.ToCommittedEvent(),
                        eventHorizonEvent.StreamSequenceNumber,
                        StreamId.EventLog,
                        Guid.Empty,
                        false),
                    cancellationToken).ConfigureAwait(false);
                return new ConsumerResponse();
            }
            catch (Exception ex)
            {
                _logger.ErrorWhileHandlingEventFromSubscription(ex, subscriptionId);
                return new ConsumerResponse
                {
                    Failure = new Failure(FailureId.Other, $"An error occurred while handling event horizon event coming from subscription {subscriptionId}. {ex.Message}")
                };
            }
        }

        static bool TryGetException(IEnumerable<Task> tasks, out Exception exception)
        {
            exception = tasks.FirstOrDefault(_ => _.Exception != default)?.Exception;
            if (exception != default)
            {
                while (exception.InnerException != null) exception = exception.InnerException;
            }

            return exception != default;
        }

        bool TryGetMicroserviceAddress(Microservice producerMicroservice, out MicroserviceAddress microserviceAddress)
        {
            var result = _microservicesConfiguration.TryGetValue(producerMicroservice, out var microserviceAddressConfig);
            microserviceAddress = microserviceAddressConfig;
            return result;
        }
    }
}
