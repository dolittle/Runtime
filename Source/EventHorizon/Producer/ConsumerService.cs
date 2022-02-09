// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Applications.Configuration;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Tenancy;
using Grpc.Core;
using static Dolittle.Runtime.EventHorizon.Contracts.Consumer;
using ProtobufContracts = Dolittle.Protobuf.Contracts;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Represents the implementation of <see cref="ConsumerBase"/>.
/// </summary>
[Singleton]
public class ConsumerService : ConsumerBase, IDisposable
{
    readonly MicroserviceId _thisMicroserviceId;
    readonly IExecutionContextManager _executionContextManager;
    readonly EventHorizonConsentsConfiguration _eventHorizonConsents;
    readonly ITenants _tenants;
    readonly FactoryFor<IEventFetchers> _getEventFetchers;
    readonly FactoryFor<IStreamEventWatcher> _getStreamWatcher;
    readonly IInitiateReverseCallServices _reverseCalls;
    readonly IConsumerProtocol _protocol;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumerService"/> class.
    /// </summary>
    /// <param name="boundedContextConfiguration">The <see cref="BoundedContextConfiguration" />.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
    /// <param name="eventHorizonConsents">The <see cref="EventHorizonConsentsConfiguration" />.</param>
    /// <param name="tenants">The <see cref="ITenants"/> system.</param>
    /// <param name="getEventFetchers">The <see cref="FactoryFor{T}" /> <see cref="IEventFetchers" />.</param>
    /// <param name="getStreamWatcher">The <see cref="FactoryFor{T}" /> <see cref="IStreamEventWatcher" />.</param>
    /// <param name="reverseCalls">The <see cref="IInitiateReverseCallServices" />.</param>
    /// <param name="protocol">The <see cref="IConsumerProtocol" />.</param>
    /// <param name="metrics">The system for capturing metrics.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public ConsumerService(
        BoundedContextConfiguration boundedContextConfiguration,
        IExecutionContextManager executionContextManager,
        EventHorizonConsentsConfiguration eventHorizonConsents,
        ITenants tenants,
        FactoryFor<IEventFetchers> getEventFetchers,
        FactoryFor<IStreamEventWatcher> getStreamWatcher,
        IInitiateReverseCallServices reverseCalls,
        IConsumerProtocol protocol,
        IMetricsCollector metrics,
        ILogger logger)
    {
        _thisMicroserviceId = boundedContextConfiguration.BoundedContext;
        _executionContextManager = executionContextManager;
        _eventHorizonConsents = eventHorizonConsents;
        _tenants = tenants;
        _getEventFetchers = getEventFetchers;
        _getStreamWatcher = getStreamWatcher;
        _reverseCalls = reverseCalls;
        _protocol = protocol;
        _metrics = metrics;
        _logger = logger;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ConsumerService"/> class.
    /// </summary>
    ~ConsumerService()
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
    public override async Task Subscribe(
        IAsyncStreamReader<EventHorizonConsumerToProducerMessage> producerStream,
        IServerStreamWriter<EventHorizonProducerToConsumerMessage> consumerStream,
        ServerCallContext context)
    {
        var token = context.CancellationToken;
        var tryConnect = await _reverseCalls.Connect(
            producerStream,
            consumerStream,
            context,
            _protocol,
            token).ConfigureAwait(false);

        if (!tryConnect.Success)
        {
            return;
        }
        var (dispatcher, arguments) = tryConnect.Result;
        _executionContextManager.CurrentFor(arguments.ExecutionContext);

        _metrics.IncrementTotalIncomingSubscriptions();
        Log.IncomingEventHorizonSubscription(
            _logger,
            arguments.ConsumerMicroserviceId,
            arguments.ConsumerTenant,
            arguments.ProducerTenant,
            arguments.StreamPosition,
            arguments.Partition,
            arguments.PublicStream);

        var subscriptionResponse = CreateSubscriptionResponse(arguments.ConsumerMicroserviceId, arguments.ConsumerTenant, arguments.ProducerTenant, arguments.PublicStream, arguments.Partition);
        if (subscriptionResponse.Failure != null)
        {
            _metrics.IncrementTotalRejectedSubscriptions();
            await dispatcher.Reject(subscriptionResponse, context.CancellationToken).ConfigureAwait(false);
            return;
        }

        _metrics.IncrementTotalAcceptedSubscriptions();
        Log.SuccessfullySubscribed(
            _logger,
            arguments.ConsumerMicroserviceId,
            arguments.ConsumerTenant,
            arguments.ProducerTenant,
            arguments.StreamPosition,
            arguments.Partition,
            arguments.PublicStream);

        using var jointCts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
        try
        {
            var tasks = new[]
            {
                dispatcher.Accept(subscriptionResponse, jointCts.Token),
                Task.Run(
                    async () => await WriteEventsToEventHorizon(
                        dispatcher,
                        arguments.ProducerTenant,
                        arguments.PublicStream,
                        arguments.Partition,
                        arguments.StreamPosition,
                        jointCts.Token).ConfigureAwait(false))
            };

            var anyTask = await Task.WhenAny(tasks).ConfigureAwait(false);
            if (!jointCts.IsCancellationRequested)
            {
                jointCts.Cancel();
            }
            if (TryGetException(tasks, out var ex))
            {
                Log.ErrorOccurredInEventHorizon(
                    _logger,
                    ex,
                    arguments.ConsumerMicroserviceId,
                    arguments.ConsumerTenant,
                    arguments.ProducerTenant,
                    arguments.Partition,
                    arguments.PublicStream);
                await Task.WhenAll(tasks).ConfigureAwait(false);
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            if (!context.CancellationToken.IsCancellationRequested)
            {
                Log.ErrorOccurredInEventHorizon(
                    _logger,
                    ex,
                    arguments.ConsumerMicroserviceId,
                    arguments.ConsumerTenant,
                    arguments.ProducerTenant,
                    arguments.Partition,
                    arguments.PublicStream);
            }

            Log.EventHorizonStopped(
                _logger,
                arguments.ConsumerMicroserviceId,
                arguments.ConsumerTenant,
                arguments.ProducerTenant,
                arguments.Partition,
                arguments.PublicStream);
        }
        catch (Exception ex)
        {
            if (!jointCts.IsCancellationRequested)
            {
                jointCts.Cancel();
            }
            if (!context.CancellationToken.IsCancellationRequested)
            {
                Log.ErrorOccurredInEventHorizon(
                    _logger,
                    ex,
                    arguments.ConsumerMicroserviceId,
                    arguments.ConsumerTenant,
                    arguments.ProducerTenant,
                    arguments.Partition,
                    arguments.PublicStream);
            }

            throw;
        }
        finally
        {
            Log.EventHorizonDisconnecting(
                _logger,
                arguments.ConsumerMicroserviceId,
                arguments.ConsumerTenant,
                arguments.ProducerTenant,
                arguments.Partition,
                arguments.PublicStream);
        }
    }

    /// <summary>
    /// Dispose resources.
    /// </summary>
    /// <param name="disposeManagedResources">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposeManagedResources)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
    }

    async Task WriteEventsToEventHorizon(
        IReverseCallDispatcher<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, SubscriptionResponse, ConsumerRequest, ConsumerResponse> dispatcher,
        TenantId producerTenant,
        StreamId publicStream,
        PartitionId partition,
        StreamPosition streamPosition,
        CancellationToken cancellationToken)
    {
        try
        {
            _executionContextManager.CurrentFor(
                _thisMicroserviceId,
                producerTenant,
                _executionContextManager.Current.CorrelationId);
            var publicEvents = await _getEventFetchers().GetPartitionedFetcherFor(
                ScopeId.Default,
                new StreamDefinition(new PublicFilterDefinition(StreamId.EventLog, publicStream)),
                cancellationToken).ConfigureAwait(false);
            var eventWaiter = _getStreamWatcher();
            while (!cancellationToken.IsCancellationRequested && !_disposed)
            {
                try
                {
                    var tryGetStreamEvent = await publicEvents.FetchInPartition(partition, streamPosition, cancellationToken).ConfigureAwait(false);
                    if (!tryGetStreamEvent.Success)
                    {
                        await eventWaiter.WaitForEvent(publicStream, streamPosition, TimeSpan.FromMinutes(1), cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    var streamEvent = tryGetStreamEvent.Result;
                    _metrics.IncrementTotalEventsWrittenToEventHorizon();
                    var response = await dispatcher.Call(
                        new ConsumerRequest { Event = streamEvent.ToEventHorizonEvent() },
                        cancellationToken).ConfigureAwait(false);
                    if (response.Failure != null)
                    {
                        Log.ErrorOccurredWhileHandlingRequest(_logger, response.Failure.Id.ToGuid(), response.Failure.Reason);
                        return;
                    }

                    streamPosition = streamEvent.Position + 1;
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

    SubscriptionResponse CreateSubscriptionResponse(MicroserviceId consumerMicroservice, TenantId consumerTenant, TenantId producerTenant, StreamId publicStream, PartitionId partition)
    {
        try
        {
            Log.CheckingIfProducerTenantExists(_logger, producerTenant);
            if (!ProducerTenantExists(producerTenant))
            {
                Log.NoConsentsConfiguredForProducerTenant(_logger, producerTenant);
                return new SubscriptionResponse { Failure = new ProtobufContracts.Failure
                {
                    Id = SubscriptionFailures.MissingConsent.ToProtobuf(),
                    Reason = $"There are no consents configured for Producer Tenant {producerTenant}",
                } };
            }

            if (!TryGetConsentFor(consumerMicroservice, consumerTenant, producerTenant, publicStream, partition, out var consentId))
            {
                Log.NoConsentsConfiguredForConsumer(_logger, partition, publicStream, producerTenant, consumerTenant, consumerMicroservice);
                return new SubscriptionResponse { Failure = new ProtobufContracts.Failure
                {
                    Id = SubscriptionFailures.MissingConsent.ToProtobuf(),
                    Reason = $"There are no consent configured for Partition {partition} in Public Stream {publicStream} in Tenant {producerTenant} to Consumer Tenant {consumerTenant} in Microservice {consumerMicroservice}"
                } };
            }

            return new SubscriptionResponse { ConsentId = consentId.ToProtobuf() };
        }
        catch (Exception ex)
        {
            Log.ErrorCreatingSubscriptionResponse(_logger, ex);
            return new SubscriptionResponse { Failure = new ProtobufContracts.Failure
            {
                Id = FailureId.Other.ToProtobuf(),
                Reason = "Error occurred while creating subscription response"
            } };
        }
    }

    bool TryGetConsentFor(MicroserviceId consumerMicroservice, TenantId consumerTenant, TenantId producerTenant, StreamId publicStream, PartitionId partition, out ConsentId consentId)
    {
        consentId = null;
        Log.CheckingConsents(
            _logger,
            partition,
            publicStream,
            producerTenant,
            consumerTenant,
            consumerMicroservice);

        var consentsForSubscription = _eventHorizonConsents
            .GetConsentConfigurationsFor(producerTenant)
            .Where(_ => _.Microservice == consumerMicroservice && _.Tenant == consumerTenant && _.Stream == publicStream && _.Partition == partition).ToArray();

        if (consentsForSubscription.Length == 0)
        {
            Log.NoConsentsConfiguredForConsumer(
                _logger,
                partition,
                publicStream,
                producerTenant,
                consumerTenant,
                consumerMicroservice);
            return false;
        }

        if (consentsForSubscription.Length > 1)
        {
            Log.MultipleConsentsConfiguredForConsumer(
                _logger,
                partition,
                publicStream,
                producerTenant,
                consumerTenant,
                consumerMicroservice);
        }

        consentId = consentsForSubscription.SingleOrDefault()?.Consent;
        return consentId != null;
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

    bool ProducerTenantExists(TenantId producerTenant) =>
        _tenants.All.Contains(producerTenant);

}
