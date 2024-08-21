// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.EventHorizon.UnBreaking;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.Clients;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections;

/// <summary>
/// Represent an implementation <see cref="IEventHorizonConnection" />. 
/// </summary>
public class EventHorizonConnection : IEventHorizonConnection
{
    readonly ExecutionContext _executionContext;
    readonly IReverseCallClient<ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> _reverseCallClient;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;
    SubscriptionId _subscriptionId;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHorizonConnection" /> class.
    /// </summary>
    /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
    /// <param name="reverseCallClient">The reverse call client.</param>
    /// <param name="metrics">The system for collecting metrics.</param>
    /// <param name="logger">The logger.</param>
    public EventHorizonConnection(
        ExecutionContext executionContext,
        IReverseCallClient<ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> reverseCallClient,
        IMetricsCollector metrics,
        ILogger logger)
    {
        _executionContext = executionContext;
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
        _metrics.IncrementTotalConnectionAttempts();
        var watch = new Stopwatch();
        watch.Start();
        if (await _reverseCallClient.Connect(CreateRequest(subscription, publicEventsPosition), _executionContext, cancellationToken).ConfigureAwait(false))
        {
            watch.Stop();
            _metrics.AddTotalTimeSpentConnecting(watch.Elapsed);
            _subscriptionId = subscription;

            var response = _reverseCallClient.ConnectResponse;
            if (response.Failure != default)
            {
                _metrics.IncrementTotalFailureResponses();
                _logger.ConnectionToProducerRuntimeFailed(subscription, response.Failure.Reason);

                if (response.Failure.Id.ToGuid() == Producer.SubscriptionFailures.MissingConsent.Value)
                {
                    _metrics.IncrementTotalSubscriptionsWithMissingConsent();
                }
                if (response.Failure.Id.ToGuid() == Producer.SubscriptionFailures.MissingSubscriptionArguments.Value)
                {
                    _metrics.IncrementTotalSubcriptionsWithMissingArguments();
                }
                return SubscriptionResponse.Failed(response.Failure);
            }
            _metrics.IncrementTotalSuccessfulResponses();
            _logger.ConnectionToProducerRuntimeSucceeded(subscription);
            return SubscriptionResponse.Succeeded(response.ConsentId.ToGuid());
        }
        watch.Stop();
        _metrics.IncrementTotalConnectionsFailed();
        _logger.CouldNotConnectToProducerRuntime(subscription);
        return SubscriptionResponse.Failed(
            new Failure(
                SubscriptionFailures.CouldNotConnectToProducerRuntime,
                "Could not connect to producer Runtime"));
    }

    /// <inheritdoc/>
    public Task StartReceivingEventsInto(
        Channel<StreamEvent> connectionToStreamProcessorQueue,
        CancellationToken cancellationToken)
    {
        return _reverseCallClient.Handle(
            (request, _, cancellationToken)
                => HandleEventFromEventHorizon(
                    connectionToStreamProcessorQueue,
                    request.Event,
                    cancellationToken),
            cancellationToken);
    }

    async Task<ConsumerResponse> HandleEventFromEventHorizon(
        Channel<StreamEvent> connectionToStreamProcessorQueue,
        EventHorizonEvent @event,
        CancellationToken cancellationToken)
    {
        try
        {
            _metrics.IncrementTotalEventHorizonEventsHandled();
            _logger.HandlingEventForSubscription(_subscriptionId);
            var streamEvent = new StreamEvent(
                @event.Event.ToCommittedEvent(),
                @event.StreamSequenceNumber,
                StreamId.EventLog,
                PartitionId.None,
                false);

            await connectionToStreamProcessorQueue.Writer.WriteAsync(streamEvent, cancellationToken).ConfigureAwait(false);
            return CreateSuccessfulResponse();
        }
        catch (Exception exception)
        {
            _metrics.IncrementTotalEventHorizonEventsFailedHandling();
            _logger.ErrorWhileHandlingEventFromSubscription(_subscriptionId, exception);
            return CreateFailureResponse(new Failure(
                SubscriptionFailures.ErrorHandlingEvent,
                exception.Message));
        }
    }

    static ConsumerSubscriptionRequest CreateRequest(SubscriptionId subscription, StreamPosition publicEventsPosition)
    {
        var request = new ConsumerSubscriptionRequest
        {
            PartitionId = subscription.PartitionId.Value,
            StreamId = subscription.StreamId.ToProtobuf(),
            StreamPosition = publicEventsPosition.Value,
            TenantId = subscription.ProducerTenantId.ToProtobuf()
        };
        request.TrySetPartitionIdLegacy();
        return request;
    }

    static ConsumerResponse CreateSuccessfulResponse() => new();

    static ConsumerResponse CreateFailureResponse(Failure failure) => new() { Failure = failure };

    public void Dispose()
    {
        _reverseCallClient?.Dispose();
    }
}
