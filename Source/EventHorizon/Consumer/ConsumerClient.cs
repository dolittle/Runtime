// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Resilience;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
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
    [Singleton]
    public class ConsumerClient : IConsumerClient
    {
        readonly IClientManager _clientManager;
        readonly ISubscriptions _subscriptions;
        readonly MicroservicesConfiguration _microservicesConfiguration;
        readonly IExecutionContextManager _executionContextManager;
        readonly FactoryFor<IStreamProcessors> _getStreamProcessors;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
        readonly FactoryFor<IWriteEventHorizonEvents> _getReceivedEventsWriter;
        readonly IAsyncPolicyFor<ConsumerClient> _policy;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerClient"/> class.
        /// </summary>
        /// <param name="clientManager">The <see cref="IClientManager" />.</param>
        /// <param name="subscriptions">The <see cref="ISubscriptions" />.</param>
        /// <param name="microservicesConfiguration">The <see cref="MicroservicesConfiguration" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="getStreamProcessors">The <see cref="FactoryFor{IStreamProcessors}" />.</param>
        /// <param name="getStreamProcessorStates">The <see cref="FactoryFor{IStreamProcessorStateRepository}" />.</param>
        /// <param name="getReceivedEventsWriter">The <see cref="FactoryFor{IWriteReceivedEvents}" />.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ConsumerClient" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ConsumerClient(
            IClientManager clientManager,
            ISubscriptions subscriptions,
            MicroservicesConfiguration microservicesConfiguration,
            IExecutionContextManager executionContextManager,
            FactoryFor<IStreamProcessors> getStreamProcessors,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
            FactoryFor<IWriteEventHorizonEvents> getReceivedEventsWriter,
            IAsyncPolicyFor<ConsumerClient> policy,
            ILogger logger)
        {
            _clientManager = clientManager;
            _subscriptions = subscriptions;
            _microservicesConfiguration = microservicesConfiguration;
            _executionContextManager = executionContextManager;
            _getStreamProcessors = getStreamProcessors;
            _getStreamProcessorStates = getStreamProcessorStates;
            _getReceivedEventsWriter = getReceivedEventsWriter;
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

            var streamProcessorStates = _getStreamProcessorStates();
            return await HandleSubscriptionResponse(
                await Subscribe(subscription, microserviceAddress, streamProcessorStates).ConfigureAwait(false),
                subscription).ConfigureAwait(false);
        }

        bool TryAddNewSubscription(Subscription subscription) =>
            _subscriptions.AddSubscription(subscription);

        bool TryGetMicroserviceAddress(Microservice producerMicroservice, out MicroserviceAddress microserviceAddress) =>
            _microservicesConfiguration.TryGetValue(producerMicroservice, out microserviceAddress);

        async Task<AsyncServerStreamingCall<grpc.SubscriptionStreamMessage>> Subscribe(Subscription subscription, MicroserviceAddress microserviceAddress, IStreamProcessorStateRepository streamProcessorStates)
        {
            _logger.Trace($"Tenant '{subscription.ConsumerTenant}' is subscribing to events from tenant '{subscription.ProducerTenant}' in microservice '{subscription.ProducerMicroservice}' on address '{microserviceAddress.Host}:{microserviceAddress.Port}'");
            var currentStreamProcessorState = await streamProcessorStates.GetOrAddNew(new StreamProcessorId(
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

        async Task<SubscriptionResponse> HandleSubscriptionResponse(AsyncServerStreamingCall<grpc.SubscriptionStreamMessage> call, Subscription subscription, IStreamProcessorStateRepository streamProcessorStates)
        {
            if (!await call.ResponseStream.MoveNext(CancellationToken.None).ConfigureAwait(false)
                || call.ResponseStream.Current.MessageCase != grpc.SubscriptionStreamMessage.MessageOneofCase.SubscriptionResponse)
            {
                var message = $"Did not receive subscription response when subscribing with subscription {subscription}";
                _logger.Warning(message);
                return new FailedSubscriptionResponse(message);
            }

            var subscriptionResponse = call.ResponseStream.Current.SubscriptionResponse;
            if (subscriptionResponse.Failure != null)
            {
                return new FailedSubscriptionResponse(subscriptionResponse.Failure.Reason);
            }

            _ = StartProcessingEventHorizon(subscription, call, streamProcessorStates);

            return new SuccessfulSubscriptionResponse();
        }

        async Task StartProcessingEventHorizon(Subscription subscription, AsyncServerStreamingCall<grpc.SubscriptionStreamMessage> call, IStreamProcessorStateRepository streamProcessorStates)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await _policy.Execute(
                async (cancellationToken) =>
                {
                    var queue = new AsyncProducerConsumerQueue<grpc.EventHorizonEvent>();
                    var eventsFetcher = new EventsFromEventHorizonFetcher(subscription, queue, _logger);
                    _getStreamProcessors().Register(
                        new EventProcessor(subscription, _getReceivedEventsWriter(), _logger),
                        eventsFetcher,
                        subscription.ProducerMicroservice.Value,
                        cancellationToken);

                    while (!cancellationToken.IsCancellationRequested
                        && await call.ResponseStream.MoveNext(cancellationToken).ConfigureAwait(false))
                    {
                        await queue.EnqueueAsync(call.ResponseStream.Current.Event).ConfigureAwait(false);
                    }
                }, cancellationTokenSource.Token).ConfigureAwait(false);
        }
    }
}