using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Represents an implementation of <see cref="IEventHorizons"/>.
/// </summary>
[Singleton]
public class EventHorizons : IEventHorizons
{
    readonly ConcurrentDictionary<EventHorizonId, EventHorizon> _eventHorizons = new();
    readonly ICreateExecutionContexts _executionContexts;
    readonly Func<TenantId, IEventFetchers> _getEventFetchers;
    readonly Func<TenantId, IStreamEventWatcher> _getStreamWaiter;
    readonly IMetricsCollector _metrics;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHorizons"/> class.
    /// </summary>
    /// <param name="executionContexts">The <see cref="ICreateExecutionContexts"/>.</param>
    /// <param name="getEventFetchers">The <see cref="Func{TResult}"/> callback for getting <see cref="IEventFetchers"/> in a tenant container.</param>
    /// <param name="getStreamWaiter">The <see cref="Func{TResult}"/> callback for getting <see cref="IStreamEventWatcher"/> in a tenant container</param>
    /// <param name="metrics">The <see cref="IMetricsCollector"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public EventHorizons(
        ICreateExecutionContexts executionContexts,
        Func<TenantId, IEventFetchers> getEventFetchers,
        Func<TenantId, IStreamEventWatcher> getStreamWaiter,
        IMetricsCollector metrics,
        ILoggerFactory loggerFactory)
    {
        _executionContexts = executionContexts;
        _getEventFetchers = getEventFetchers;
        _getStreamWaiter = getStreamWaiter;
        _metrics = metrics;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<EventHorizons>();
    }

    /// <inheritdoc />
    public async Task Start(
        IReverseCallDispatcher<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, SubscriptionResponse, ConsumerRequest, ConsumerResponse> dispatcher,
        ConsumerSubscriptionArguments arguments,
        ConsentId consent,
        CancellationToken cancellationToken)
    {
        var eventHorizonId = new EventHorizonId(
            arguments.ConsumerMicroservice,
            arguments.ConsumerTenant,
            arguments.ProducerTenant,
            arguments.PublicStream,
            arguments.Partition);

        using var eventHorizon = new EventHorizon(
            eventHorizonId,
            consent,
            arguments.StreamPosition,
            dispatcher,
            _executionContexts,
            _getEventFetchers,
            _getStreamWaiter,
            _metrics,
            _loggerFactory.CreateLogger<EventHorizon>(),
            cancellationToken);

        if (!_eventHorizons.TryAdd(eventHorizonId, eventHorizon))
        {
            Log.EventHorizonAlreadyRegistered(
                _logger,
                eventHorizonId.ConsumerMicroservice,
                eventHorizonId.ConsumerTenant,
                eventHorizonId.ProducerTenant,
                eventHorizonId.Partition,
                eventHorizonId.PublicStream);
            await RejectAlreadyRegisteredEventHorizon(dispatcher, eventHorizonId, cancellationToken).ConfigureAwait(false);
            return;
        }
        try
        {
            await eventHorizon.Start().ConfigureAwait(false);
        }
        finally
        {
            _eventHorizons.Remove(eventHorizonId, out _);
        }
    }

    Task RejectAlreadyRegisteredEventHorizon(
        IReverseCallDispatcher<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, SubscriptionResponse, ConsumerRequest, ConsumerResponse> dispatcher,
        EventHorizonId eventHorizonId,
        CancellationToken cancellationToken)
    {
        var rejection = new SubscriptionResponse()
        {
            Failure = new EventHorizonAlreadyRegistered(eventHorizonId),
        };
        return dispatcher.Reject(rejection, cancellationToken);
    }
}
