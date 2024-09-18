// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer.Connections;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Microservices;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents an implementation of <see cref="ISubscription" />.
/// </summary>
public class Subscription : ISubscription
{
    readonly object _startLock = new();
    readonly object _responseLock = new();
    readonly MicroserviceAddress _connectionAddress;
    readonly ExecutionContext _executionContext;
    readonly ISubscriptionPolicies _policies;
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
    /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
    /// <param name="policies">The policy to use for handling the <see cref="SubscribeLoop(CancellationToken)"/>.</param>
    /// <param name="connectionFactory">The factory to use for creating new connections to the producer Runtime.</param>
    /// <param name="streamProcessorFactory">The factory to use for creating stream processors that write the received events.</param>
    /// <param name="subscriptionPositions">The system to use for getting the next event to recieve for a subscription.</param>
    /// <param name="metrics">The system for collecting metrics.</param>
    /// <param name="processingMetrics">The <see cref="Processing.IMetricsCollector"/>.</param>
    /// <param name="logger">The system for logging messages.</param>
    public Subscription(
        SubscriptionId identifier,
        MicroserviceAddress connectionAddress,
        ExecutionContext executionContext,
        ISubscriptionPolicies policies,
        IEventHorizonConnectionFactory connectionFactory,
        IStreamProcessorFactory streamProcessorFactory,
        IGetNextEventToReceiveForSubscription subscriptionPositions,
        IMetricsCollector metrics,
        Processing.IMetricsCollector processingMetrics,
        ILogger logger)
    {
        Identifier = identifier;
        _connectionAddress = connectionAddress;
        _executionContext = executionContext;
        _policies = policies;
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
            _connectionResponse = new TaskCompletionSource<SubscriptionResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
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
        return _policies.Connecting.ExecuteAsync(SubscribeLoop, CancellationToken.None);
    }

    async Task SubscribeLoop(CancellationToken cancellationToken)
    {
        _metrics.IncrementSubscriptionLoops();
        _logger.SubscriptionLoopStarting(Identifier, State);

        try
        {
            var connectionAndResponse = await ConnectToEventHorizon(cancellationToken).ConfigureAwait(false);
            var response = connectionAndResponse.Item2;
            using var connection = connectionAndResponse.Item1;
            if (response.Success)
            {
                _metrics.IncrementCurrentConnectedSubscriptions();
                await ReceiveAndWriteEvents(connection, response.ConsentId, cancellationToken).ConfigureAwait(false);
                throw new SubscriptionLoopCompleted(Identifier);
            }
            else
            {
                _logger.SubscriptionFailedToConnectToProducerRuntime(Identifier, response.Failure);
                throw new CouldNotConnectToProducerRuntime(Identifier, response.Failure);
            }
        }
        finally
        {
            EnsureConnectionResponseIsUnresolved();
        }
    }
    async Task<(IEventHorizonConnection, SubscriptionResponse)> ConnectToEventHorizon(CancellationToken cancellationToken)
    {
        try
        {
            State = SubscriptionState.Connecting;

            var connection = _connectionFactory.Create(_connectionAddress, _executionContext);
            var nextEventToReceive = await _subscriptionPositions.GetNextEventToReceiveFor(Identifier, cancellationToken);
            var response = await connection.Connect(Identifier, nextEventToReceive, cancellationToken).ConfigureAwait(false);
            SetConnectionResponse(response);
            return (connection, response);
        }
        catch (Exception exception)
        {
            _logger.SubscriptionFailedWithException(Identifier, exception);
            SetConnectionResponse(
                SubscriptionResponse.Failed(
                    new Failure(SubscriptionFailures.CouldNotConnectToProducerRuntime, exception.Message)));
            throw;
        }
    }
    async Task ReceiveAndWriteEvents(IEventHorizonConnection connection, ConsentId consent, CancellationToken cancellationToken)
    {
        try
        {
            _logger.SubscriptionIsReceivingAndWriting(Identifier, consent);
            using var processingCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var connectionToStreamProcessorQueue = Channel.CreateBounded<StreamEvent>(1000);
            var writeEventsStreamProcessor = _streamProcessorFactory.Create(consent, Identifier, _executionContext, new EventsFromEventHorizonFetcher(connectionToStreamProcessorQueue, _processingMetrics));
            var tasks = new TaskGroup(
                writeEventsStreamProcessor.StartAndWait(processingCancellationToken.Token),
                connection.StartReceivingEventsInto(connectionToStreamProcessorQueue, processingCancellationToken.Token)
            );

            State = SubscriptionState.Connected;
            await tasks.WaitForAllCancellingOnFirst(processingCancellationToken).ConfigureAwait(false);
            _logger.SubsciptionFailedWhileReceivingAndWriting(Identifier, consent, null);
            _metrics.IncrementSubscriptionsFailedDueToReceivingOrWritingEventsCompleted();
        }
        catch (Exception ex)
        {
            _logger.SubsciptionFailedWhileReceivingAndWriting(Identifier, consent, ex);
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
            _connectionResponse.TrySetCanceled();
            CreateUnresolvedConnectionResponse();
        }
    }

}
