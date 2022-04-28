// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Represents an an event horizon in the system.
/// </summary>
public class EventHorizon : IDisposable
{
    readonly IReverseCallDispatcher<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, SubscriptionResponse, ConsumerRequest, ConsumerResponse> _dispatcher;
    readonly ICreateExecutionContexts _executionContexts;
    readonly Func<TenantId, IEventFetchers> _getEventFetchers;
    readonly Func<TenantId, IStreamEventWatcher> _getStreamWaiter;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;
    readonly CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHorizon"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EventHorizonId"/>.</param>
    /// <param name="consent">The <see cref="ConsentId"/>.</param>
    /// <param name="currentPosition">The initial current <see cref="StreamPosition"/> of the event horizon.</param>
    /// <param name="dispatcher">The reverse call dispatcher.</param>
    /// <param name="executionContexts">The <see cref="ICreateExecutionContexts"/>.</param>
    /// <param name="getEventFetchers">The <see cref="Func{TResult}"/> callback for getting <see cref="IEventFetchers"/> in a tenant container.</param>
    /// <param name="getStreamWaiter">The <see cref="Func{TResult}"/> callback for getting <see cref="IStreamEventWatcher"/> in a tenant container</param>
    /// <param name="metrics">The <see cref="IMetricsCollector"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public EventHorizon(
        EventHorizonId id,
        ConsentId consent,
        StreamPosition currentPosition,
        IReverseCallDispatcher<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, SubscriptionResponse, ConsumerRequest, ConsumerResponse> dispatcher,
        ICreateExecutionContexts executionContexts,
        Func<TenantId, IEventFetchers> getEventFetchers,
        Func<TenantId, IStreamEventWatcher> getStreamWaiter,
        IMetricsCollector metrics,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        Consent = consent;
        _dispatcher = dispatcher;
        _executionContexts = executionContexts;
        _getEventFetchers = getEventFetchers;
        _getStreamWaiter = getStreamWaiter;
        _metrics = metrics;
        _logger = logger;
        Id = id;
        CurrentPosition = currentPosition;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }
    
    /// <summary>
    /// Gets the <see cref="ConsentId"/>.
    /// </summary>
    public ConsentId Consent { get; }
    
    /// <summary>
    /// Gets the <see cref="EventHorizonId"/>.
    /// </summary>
    public EventHorizonId Id { get; }

    /// <summary>
    /// Gets the <see cref="StreamPosition"/>.
    /// </summary>
    public StreamPosition CurrentPosition { get; private set; }

    /// <summary>
    /// Starts the event horizon.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Start()
    {
        try
        {
            var tasks = new TaskGroup(
                _dispatcher.Accept(new SubscriptionResponse{ ConsentId = Consent.ToProtobuf() }, _cancellationTokenSource.Token),
                ProcessEventsThroughEventHorizon());
            tasks.OnFirstTaskFailure += (_, ex) => Log.ErrorOccurredInEventHorizon(
                _logger,
                ex,
                Id.ConsumerMicroservice,
                Id.ConsumerTenant,
                Id.ProducerTenant,
                Id.Partition,
                Id.PublicStream);;
            tasks.OnAllTasksCompleted += () => Log.EventHorizonStopped(
                _logger,
                Id.ConsumerMicroservice,
                Id.ConsumerTenant,
                Id.ProducerTenant,
                Id.Partition,
                Id.PublicStream);
            await tasks.WaitForAllCancellingOnFirst(_cancellationTokenSource).ConfigureAwait(false);
        }
        finally
        {
            Log.EventHorizonDisconnecting(
                _logger,
                Id.ConsumerMicroservice,
                Id.ConsumerTenant,
                Id.ProducerTenant,
                Id.Partition,
                Id.PublicStream);
        }
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        _dispatcher?.Dispose();
        _cancellationTokenSource?.Dispose();
    }

    async Task ProcessEventsThroughEventHorizon()
    {
        try
        {
            var publicEvents = await _getEventFetchers(Id.ProducerTenant).GetPartitionedFetcherFor(
                ScopeId.Default,
                new StreamDefinition(new PublicFilterDefinition(StreamId.EventLog, Id.PublicStream)),
                _cancellationTokenSource.Token).ConfigureAwait(false);
            var eventWaiter = _getStreamWaiter(Id.ProducerTenant);
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var tryGetStreamEvent = await publicEvents.FetchInPartition(
                        Id.Partition,
                        CurrentPosition,
                        _cancellationTokenSource.Token).ConfigureAwait(false);
                    if (!tryGetStreamEvent.Success)
                    {
                        await eventWaiter.WaitForEvent(
                            Id.PublicStream,
                            CurrentPosition,
                            TimeSpan.FromMinutes(1),
                            _cancellationTokenSource.Token).ConfigureAwait(false);
                        continue;
                    }

                    var streamEvents = tryGetStreamEvent.Result;
                    foreach (var streamEvent in streamEvents)
                    {
                        _metrics.IncrementTotalEventsWrittenToEventHorizon();
                        var response = await _dispatcher.Call(
                            new ConsumerRequest { Event = streamEvent.ToEventHorizonEvent() },
                            _executionContexts.TryCreateUsing(streamEvent.Event.ExecutionContext),
                            _cancellationTokenSource.Token).ConfigureAwait(false);
                        if (response.Failure != null)
                        {
                            Log.ErrorOccurredWhileHandlingRequest(_logger, response.Failure.Id.ToGuid(), response.Failure.Reason);
                            return;
                        }

                        CurrentPosition = streamEvent.Position + 1;
                    }
                }
                catch (EventStoreUnavailable)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            Log.ErrorWritingEventToEventHorizon(_logger, ex);
        }
    }
}
