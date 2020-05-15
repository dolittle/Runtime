// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.ApplicationModel;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Resilience;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Services.Clients;
using Grpc.Core;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IConsumerClient" />.
    /// </summary>
    [SingletonPerTenant]
    public class ConsumerClient : IConsumerClient
    {
        readonly IClientManager _clientManager;
        readonly ISubscriptions _subscriptions;
        readonly MicroservicesConfiguration _microservicesConfiguration;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IWriteEventHorizonEvents _eventHorizonEventsWriter;
        readonly IAsyncPolicyFor<ConsumerClient> _policy;
        readonly IExecutionContextManager _executionContextManager;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly CancellationToken _cancellationToken;
        readonly ILogger _logger;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerClient"/> class.
        /// </summary>
        /// <param name="clientManager">The <see cref="IClientManager" />.</param>
        /// <param name="subscriptions">The <see cref="ISubscriptions" />.</param>
        /// <param name="microservicesConfiguration">The <see cref="MicroservicesConfiguration" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="eventHorizonEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ConsumerClient" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ConsumerClient(
            IClientManager clientManager,
            ISubscriptions subscriptions,
            MicroservicesConfiguration microservicesConfiguration,
            IStreamProcessorStateRepository streamProcessorStates,
            IWriteEventHorizonEvents eventHorizonEventsWriter,
            IAsyncPolicyFor<ConsumerClient> policy,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _clientManager = clientManager;
            _subscriptions = subscriptions;
            _microservicesConfiguration = microservicesConfiguration;
            _streamProcessorStates = streamProcessorStates;
            _eventHorizonEventsWriter = eventHorizonEventsWriter;
            _policy = policy;
            _executionContextManager = executionContextManager;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
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
            if (_subscriptions.HasSubscription(subscriptionId))
            {
                _logger.Trace($"Already subscribed to subscription {subscriptionId}");
                return new SuccessfulSubscriptionResponse();
            }

            _logger.Trace($"Getting microservice address");
            if (!TryGetMicroserviceAddress(subscriptionId.ProducerMicroserviceId, out var microserviceAddress))
            {
                var message = $"There is no microservice configuration for the producer microservice '{subscriptionId.ProducerMicroserviceId}'.";
                _logger.Warning(message);
                return new FailedSubscriptionResponse(new Failure(SubscriptionFailures.MissingMicroserviceConfiguration, message));
            }

            return await _policy.Execute(
                async _ =>
                {
                    var call = await Subscribe(subscriptionId, microserviceAddress).ConfigureAwait(false);

                    var response = await HandleSubscriptionResponse(
                        call.ResponseStream,
                        subscriptionId).ConfigureAwait(false);

                    if (response.Success) StartProcessingEventHorizon(subscriptionId, microserviceAddress, call.ResponseStream);
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

        async Task<AsyncServerStreamingCall<Contracts.SubscriptionMessage>> Subscribe(SubscriptionId subscriptionId, MicroserviceAddress microserviceAddress)
        {
            _logger.Debug($"Tenant '{subscriptionId.ConsumerTenantId}' is subscribing to events from tenant '{subscriptionId.ProducerTenantId}' in microservice '{subscriptionId.ProducerMicroserviceId}' on address '{microserviceAddress.Host}:{microserviceAddress.Port}'");

            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(subscriptionId, CancellationToken.None).ConfigureAwait(false);
            var publicEventsPosition = tryGetStreamProcessorState.Result?.Position ?? StreamPosition.Start;

            return _clientManager
                .Get<Contracts.Consumer.ConsumerClient>(microserviceAddress.Host, microserviceAddress.Port)
                .Subscribe(
                    new Contracts.ConsumerSubscription
                    {
                        TenantId = subscriptionId.ProducerTenantId.ToProtobuf(),
                        StreamPosition = publicEventsPosition.Value,
                        StreamId = subscriptionId.StreamId.ToProtobuf(),
                        PartitionId = subscriptionId.PartitionId.ToProtobuf(),
                        CallContext = new Dolittle.Services.Contracts.CallRequestContext { ExecutionContext = _executionContextManager.Current.ToProtobuf() }
                    }, cancellationToken: _cancellationToken);
        }

        async Task<SubscriptionResponse> HandleSubscriptionResponse(IAsyncStreamReader<Contracts.SubscriptionMessage> responseStream, SubscriptionId subscriptionId)
        {
            if (!await responseStream.MoveNext(_cancellationToken).ConfigureAwait(false)
                || responseStream.Current.MessageCase != Contracts.SubscriptionMessage.MessageOneofCase.SubscriptionResponse)
            {
                var message = $"Did not receive subscription response when subscribing with subscription {subscriptionId}";
                _logger.Warning(message);
                return new FailedSubscriptionResponse(new Failure(SubscriptionFailures.DidNotReceiveSubscriptionResponse, message));
            }

            var subscriptionResponse = responseStream.Current.SubscriptionResponse;
            if (subscriptionResponse.Failure != null)
            {
                _logger.Warning($"Failed subscribing with subscription {subscriptionId}. {subscriptionResponse.Failure.Reason}");
                return new FailedSubscriptionResponse(subscriptionResponse.Failure);
            }

            _logger.Trace($"Subscription response for subscription {subscriptionId} was successful");
            return new SuccessfulSubscriptionResponse();
        }

        void StartProcessingEventHorizon(SubscriptionId subscriptionId, MicroserviceAddress microserviceAddress, IAsyncStreamReader<Contracts.SubscriptionMessage> responseStream)
        {
            Task.Run(async () =>
                {
                    try
                    {
                        await ReadEventsFromEventHorizon(subscriptionId, responseStream).ConfigureAwait(false);
                        throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug(ex, "Reconnecting to event horizon with subscription {subscriptionId}", subscriptionId);
                        await _policy.Execute(
                            async _ =>
                            {
                                var call = await Subscribe(subscriptionId, microserviceAddress).ConfigureAwait(false);
                                var response = await HandleSubscriptionResponse(call.ResponseStream, subscriptionId).ConfigureAwait(false);
                                if (!response.Success) throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values
                                await ReadEventsFromEventHorizon(subscriptionId, call.ResponseStream).ConfigureAwait(false);
                                throw new Todo(); // TODO: This is a hack to get the policy going. Remove this when we can have policies on return values
                            }, _cancellationToken).ConfigureAwait(false);
                    }
                });
        }

        async Task ReadEventsFromEventHorizon(SubscriptionId subscriptionId, IAsyncStreamReader<Contracts.SubscriptionMessage> responseStream)
        {
            _logger.Information("Successfully connected event horizon with {subscriptionId}. Waiting for events to process", subscriptionId);
            var queue = new AsyncProducerConsumerQueue<StreamEvent>();
            var eventsFetcher = new EventsFromEventHorizonFetcher(queue);

            using var internalCancellationSource = new CancellationTokenSource();
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(internalCancellationSource.Token, _cancellationToken);
            var cancellationToken = linkedTokenSource.Token;
            var tasks = new List<Task>();
            try
            {
                _subscriptions.TrySubscribe(
                    subscriptionId,
                    new EventProcessor(subscriptionId, _eventHorizonEventsWriter, _logger),
                    eventsFetcher,
                    cancellationToken,
                    out var outputtedStreamProcessor);
                using var streamProcessor = outputtedStreamProcessor;
                await streamProcessor.Initialize().ConfigureAwait(false);

                tasks.Add(Task.Run(async () =>
                    {
                        await HandleEventHorizonResponses(subscriptionId, responseStream, queue, cancellationToken).ConfigureAwait(false);
                        internalCancellationSource.Cancel();
                    }));
                tasks.Add(streamProcessor.Start());
            }
            catch (Exception ex)
            {
                internalCancellationSource.Cancel();
                _logger.Warning(ex, "Error occurred while initializing Subscription: {subscriptionId}", subscriptionId);
                return;
            }

            var finishedTask = await Task.WhenAny(tasks).ConfigureAwait(false);
            if (!internalCancellationSource.IsCancellationRequested) internalCancellationSource.Cancel();
            if (TryGetException(tasks, out var exception))
            {
                internalCancellationSource.Cancel();
                _logger.Warning(exception, "Error occurred while processing Subscription: {subscriptionId}", subscriptionId);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        async Task HandleEventHorizonResponses(
            SubscriptionId subscriptionId,
            IAsyncStreamReader<Contracts.SubscriptionMessage> responseStream,
            AsyncProducerConsumerQueue<StreamEvent> queue,
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested
                && await responseStream.MoveNext(cancellationToken).ConfigureAwait(false))
            {
                if (responseStream.Current.MessageCase != Contracts.SubscriptionMessage.MessageOneofCase.Event)
                {
                    _logger.Warning("Expected the response to contain an event in subscription {subscription}. Getting next response", subscriptionId);
                    continue;
                }

                var @event = responseStream.Current.Event;
                await queue.EnqueueAsync(
                    new StreamEvent(
                        @event.ToCommittedEvent(subscriptionId.ProducerMicroserviceId, subscriptionId.ConsumerTenantId),
                        @event.StreamSequenceNumber,
                        StreamId.EventLog,
                        Guid.Empty,
                        false),
                    cancellationToken).ConfigureAwait(false);
            }
        }

        bool TryGetException(IEnumerable<Task> tasks, out Exception exception)
        {
            exception = tasks.FirstOrDefault(_ => _.Exception != default)?.Exception;
            if (exception != default)
            {
                while (exception.InnerException != null) exception = exception.InnerException;
            }

            return exception != default;
        }

        bool TryGetMicroserviceAddress(Microservice producerMicroservice, out MicroserviceAddress microserviceAddress) =>
            _microservicesConfiguration.TryGetValue(producerMicroservice, out microserviceAddress);
    }
}