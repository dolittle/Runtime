// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer.Connections;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resilience;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscription" />.
    /// </summary>
    public class Subscription : ISubscription
    {
        readonly object _startLock = new();
        readonly object _responseLock = new();
        readonly MicroserviceAddress _connectionAddress;
        readonly IAsyncPolicyFor<Subscription> _policy;
        readonly IEventHorizonConnectionFactory _connectionFactory;
        readonly IStreamProcessorFactory _streamProcessorFactory;
        readonly IGetNextEventToReceiveForSubscription _subscriptionPositions;
        readonly IMetricsCollector _metrics;
        readonly Processing.IMetricsCollector _processingMetrics;
        readonly ILogger _logger;
        TaskCompletionSource<SubscriptionResponse> _connectionResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="identifier">The identifier of the subscription.</param>
        /// <param name="connectionAddress">The address of the producer Runtime to connect to.</param>
        /// <param name="policy">The policy to use for handling the <see cref="SubscribeLoop(CancellationToken)"/>.</param>
        /// <param name="connectionFactory">The factory to use for creating new connections to the producer Runtime.</param>
        /// <param name="streamProcessorFactory">The factory to use for creating stream processors that write the received events.</param>
        /// <param name="subscriptionPositions">The system to use for getting the next event to recieve for a subscription.</param>
        /// <param name="metrics">The system for collecting metrics.</param>
        /// <param name="logger">The system for logging messages.</param>
        public Subscription(
            SubscriptionId identifier,
            MicroserviceAddress connectionAddress,
            IAsyncPolicyFor<Subscription> policy,
            IEventHorizonConnectionFactory connectionFactory,
            IStreamProcessorFactory streamProcessorFactory,
            IGetNextEventToReceiveForSubscription subscriptionPositions,
            IMetricsCollector metrics,
            Processing.IMetricsCollector processingMetrics,
            ILogger logger)
        {
            Identifier = identifier;
            _connectionAddress = connectionAddress;
            _policy = policy;
            _connectionFactory = connectionFactory;
            _streamProcessorFactory = streamProcessorFactory;
            _subscriptionPositions = subscriptionPositions;
            _metrics = metrics;
            _processingMetrics = processingMetrics;
            _logger = logger;
            CreateUnresolvedConnectionResponse();
        }

        public SubscriptionId Identifier { get; }

        public SubscriptionState State { get; private set; } = SubscriptionState.Created;

        public Task<SubscriptionResponse> ConnectionResponse
        {
            get
            {
                lock (_responseLock)
                {
                    return _connectionResponse.Task;
                }
            }
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (HasAlreadyStarted())
            {
                _logger.SubscriptionAlreadyStarted(Identifier, State);
                return;
            }

            Task.Run(StartSubscribeLoopWithPolicy);
        }

        void CreateUnresolvedConnectionResponse()
        {
            lock (_responseLock)
            {
                _connectionResponse = new(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }

        bool HasAlreadyStarted()
            => State != SubscriptionState.Created;

        Task StartSubscribeLoopWithPolicy()
        {
            lock (_startLock)
            {
                if (HasAlreadyStarted())
                {
                    _logger.SubscriptionAlreadyStarted(Identifier, State);
                    return Task.CompletedTask;
                }

                State = SubscriptionState.Connecting;
            }

            _logger.SubscriptionStarting(Identifier);
            return _policy.Execute(SubscribeLoop, CancellationToken.None);
        }

        async Task SubscribeLoop(CancellationToken cancellationToken)
        {
            try
            {
                _metrics.IncrementSubscriptionLoops();
                _logger.SubscriptionLoopStarting(Identifier, State);

                State = SubscriptionState.Connecting;
                EnsureConnectionResponseIsUnresolved();

                var connection = _connectionFactory.Create(_connectionAddress);
                var nextEventToReceive = await _subscriptionPositions.GetNextEventToReceiveFor(Identifier, cancellationToken);
                var connectionResponse = await connection.Connect(Identifier, nextEventToReceive, cancellationToken).ConfigureAwait(false);

                SetConnectionResponse(connectionResponse);

                if (connectionResponse.Success)
                {
                    State = SubscriptionState.Connected;
                    _metrics.IncrementCurrentConnectedSubscriptions();
                    await ReceiveAndWriteEvents(connection, connectionResponse.ConsentId, cancellationToken).ConfigureAwait(false);
                    throw new SubscriptionLoopCompleted(Identifier);
                }
                else
                {
                    State = SubscriptionState.FailedToConnect;
                    _logger.SubscriptionFailedToConnectToProducerRuntime(Identifier, connectionResponse.Failure);
                    throw new CouldNotConnectToProducerRuntime(Identifier, connectionResponse.Failure);
                }
            }
            catch (Exception exception)
            {
                SetConnectionResponse(
                    SubscriptionResponse.Failed(
                        new Failure(SubscriptionFailures.CouldNotConnectToProducerRuntime, exception.Message)));
                State = SubscriptionState.FailedToConnect;
                _logger.SubscriptionFailedWithException(Identifier, exception);
                throw;
            }
        }

        async Task ReceiveAndWriteEvents(IEventHorizonConnection connection, ConsentId consent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.SubscriptionIsReceivingAndWriting(Identifier, consent);

                using var processingCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                var connectionToStreamProcessorQueue = new AsyncProducerConsumerQueue<StreamEvent>();

                var writeEventsStreamProcessor = _streamProcessorFactory.Create(consent, Identifier, new EventsFromEventHorizonFetcher(connectionToStreamProcessorQueue, _processingMetrics));

                var tasks = new[]
                {
                    writeEventsStreamProcessor.StartAndWait(
                        processingCancellationToken.Token),
                    connection.StartReceivingEventsInto(
                        connectionToStreamProcessorQueue,
                        processingCancellationToken.Token),
                };

                await Task.WhenAny(tasks).ConfigureAwait(false);

                processingCancellationToken.Cancel();
                State = SubscriptionState.FailedToProcess;
                _logger.SubsciptionFailedWhileReceivingAndWriting(Identifier, consent);

                await Task.WhenAll(tasks).ConfigureAwait(false);
                _metrics.IncrementSubscriptionsFailedDueToReceivingOrWritingEventsCompleted();
            }
            catch
            {
                _metrics.IncrementSubscriptionsFailedDueToException();
                throw;
            }
            finally
            {
                _metrics.DecrementCurrentConnectedSubscriptions();
            }
        }

        void SetConnectionResponse(SubscriptionResponse response)
        {
            lock (_responseLock)
            {
                _connectionResponse.SetResult(response);
            }
        }

        void EnsureConnectionResponseIsUnresolved()
        {
            lock (_responseLock)
            {
                if (_connectionResponse.Task.IsCompleted)
                {
                    _connectionResponse.TrySetCanceled();
                    CreateUnresolvedConnectionResponse();
                }
            }
        }

    }
}
