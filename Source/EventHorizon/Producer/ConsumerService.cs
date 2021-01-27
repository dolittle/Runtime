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
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Tenancy;
using Grpc.Core;
using static Dolittle.Runtime.EventHorizon.Contracts.Consumer;
using EventHorizonContracts = Dolittle.Runtime.EventHorizon.Contracts;
using ProtobufContracts = Dolittle.Protobuf.Contracts;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents the implementation of <see cref="ConsumerBase"/>.
    /// </summary>
    [Singleton]
    public class ConsumerService : ConsumerBase, IDisposable
    {
        readonly Microservice _thisMicroservice;
        readonly IExecutionContextManager _executionContextManager;
        readonly EventHorizonConsentsConfiguration _eventHorizonConsents;
        readonly ITenants _tenants;
        readonly FactoryFor<IEventFetchers> _getEventFetchers;
        readonly FactoryFor<IStreamEventWatcher> _getStreamWatcher;
        readonly IReverseCallDispatchers _dispatchers;
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
        /// <param name="dispatchers">The <see cref="IReverseCallDispatchers" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ConsumerService(
            BoundedContextConfiguration boundedContextConfiguration,
            IExecutionContextManager executionContextManager,
            EventHorizonConsentsConfiguration eventHorizonConsents,
            ITenants tenants,
            FactoryFor<IEventFetchers> getEventFetchers,
            FactoryFor<IStreamEventWatcher> getStreamWatcher,
            IReverseCallDispatchers dispatchers,
            ILogger<ConsumerService> logger)
        {
            _thisMicroservice = boundedContextConfiguration.BoundedContext;
            _executionContextManager = executionContextManager;
            _eventHorizonConsents = eventHorizonConsents;
            _tenants = tenants;
            _getEventFetchers = getEventFetchers;
            _getStreamWatcher = getStreamWatcher;
            _dispatchers = dispatchers;
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
            var dispatcher = _dispatchers.GetFor<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse>(
                producerStream,
                consumerStream,
                context,
                message => message.SubscriptionRequest,
                (message, response) => message.SubscriptionResponse = response,
                (message, request) => message.Request = request,
                message => message.Response,
                request => request.CallContext,
                (request, context) => request.CallContext = context,
                response => response.CallContext,
                (message, ping) => message.Ping = ping,
                message => message.Pong);

            if (!await dispatcher.ReceiveArguments(context.CancellationToken).ConfigureAwait(false))
            {
                const string message = "Event Horizon subscription arguments were not received";
                _logger.Warning(message);
                await dispatcher.Reject(
                    new SubscriptionResponse
                        {
                            Failure = new Failure(SubscriptionFailures.MissingSubscriptionArguments, message)
                        }, context.CancellationToken).ConfigureAwait(false);
                return;
            }

            var arguments = dispatcher.Arguments;
            _executionContextManager.CurrentFor(arguments.CallContext.ExecutionContext);
            var consumerMicroservice = _executionContextManager.Current.Microservice;
            var consumerTenant = _executionContextManager.Current.Tenant;
            TenantId producerTenant = arguments.TenantId.ToGuid();
            StreamId publicStream = arguments.StreamId.ToGuid();
            PartitionId partition = arguments.PartitionId.ToGuid();
            var streamPosition = arguments.StreamPosition;

            _logger.Debug(
                "Incomming Event Horizon subscription from microservice '{ConsumerMicroservice}' and tenant '{ConsumerTenant}' to tenant '{ProducerTenant}' starting at position '{StreamPosition}' in partition '{Partition}' in stream '{PublicStream}'",
                consumerMicroservice,
                consumerTenant,
                producerTenant,
                streamPosition,
                partition,
                publicStream);

            var subscriptionResponse = CreateSubscriptionResponse(consumerMicroservice, consumerTenant, producerTenant, publicStream, partition);
            if (subscriptionResponse.Failure != null)
            {
                await dispatcher.Reject(subscriptionResponse, context.CancellationToken).ConfigureAwait(false);
                return;
            }

            _logger.Information(
                "Microservice '{ConsumerMicroservice}' and tenant '{ConsumerTenant}' successfully subscribed to tenant '{ProducerTenant}' starting at position '{StreamPosition}' in partition '{Partition}' in stream '{PublicStream}'",
                consumerMicroservice,
                consumerTenant,
                producerTenant,
                streamPosition,
                partition,
                publicStream);

            using var jointCts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
            try
            {
                var tasks = new[]
                    {
                        dispatcher.Accept(subscriptionResponse, jointCts.Token),
                        Task.Run(
                            async () => await WriteEventsToEventHorizon(
                                dispatcher,
                                producerTenant,
                                publicStream,
                                partition,
                                streamPosition,
                                jointCts.Token).ConfigureAwait(false))
                    };

                var anyTask = await Task.WhenAny(tasks).ConfigureAwait(false);
                if (!jointCts.IsCancellationRequested) jointCts.Cancel();
                if (TryGetException(tasks, out var ex))
                {
                    _logger.Warning(
                        ex,
                        "An error occurred Event Horizon for Microservice '{ConsumerMicroservice}' and tenant '{ConsumerTenant}' with producer tenant '{ProducerTenant}' in partition '{Partition}' in stream '{PublicStream}'",
                        consumerMicroservice,
                        consumerTenant,
                        producerTenant,
                        streamPosition,
                        partition,
                        publicStream);
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    _logger.Warning(
                        ex,
                        "Event Horizon for microservice '{ConsumerMicroservice}' and tenant '{ConsumerTenant}' with producer tenant '{ProducerTenant}' in partition '{Partition}' in stream '{PublicStream}' failed",
                        consumerMicroservice,
                        consumerTenant,
                        producerTenant,
                        partition,
                        publicStream);
                }

                _logger.Warning(
                    "Event Horizon for microservice '{ConsumerMicroservice}' and tenant '{ConsumerTenant}' with producer tenant '{ProducerTenant}' in partition '{Partition}' in stream '{PublicStream}' stopped",
                    consumerMicroservice,
                    consumerTenant,
                    producerTenant,
                    partition,
                    publicStream);
            }
            catch (Exception ex)
            {
                if (!jointCts.IsCancellationRequested) jointCts.Cancel();
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    _logger.Warning(
                        ex,
                        "Error occurred in Event Horizon for microservice '{ConsumerMicroservice}' and tenant '{ConsumerTenant}' with producer tenant '{ProducerTenant}' in partition '{Partition}' in stream '{PublicStream}'",
                        consumerMicroservice,
                        consumerTenant,
                        producerTenant,
                        partition,
                        publicStream);
                }

                throw;
            }
            finally
            {
                _logger.Debug(
                    "Disconnecting Event Horizon for microservice '{ConsumerMicroservice}' and tenant '{ConsumerTenant}' with producer tenant '{ProducerTenant}' in partition '{Partition}' in stream '{PublicStream}'",
                    consumerMicroservice,
                    consumerTenant,
                    producerTenant,
                    partition,
                    publicStream);
            }
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposeManagedResources">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_disposed) return;

            _disposed = true;
        }

        async Task WriteEventsToEventHorizon(
            IReverseCallDispatcher<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse> dispatcher,
            TenantId producerTenant,
            StreamId publicStream,
            PartitionId partition,
            StreamPosition streamPosition,
            CancellationToken cancellationToken)
        {
            try
            {
                _executionContextManager.CurrentFor(
                    _thisMicroservice,
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
                        var response = await dispatcher.Call(
                            new ConsumerRequest { Event = streamEvent.ToEventHorizonEvent() },
                            cancellationToken).ConfigureAwait(false);
                        if (response.Failure != null)
                        {
                            _logger.Warning(
                                "An error occurred while handling request. FailureId: {FailureId} Reason: {Reason}",
                                response.Failure.Id,
                                response.Failure.Reason);
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
                _logger.Warning(ex, "An error ocurred while writing events to event horizon");
            }
        }

        EventHorizonContracts.SubscriptionResponse CreateSubscriptionResponse(Microservice consumerMicroservice, TenantId consumerTenant, TenantId producerTenant, StreamId publicStream, PartitionId partition)
        {
            try
            {
                _logger.Trace("Checking whether Producer Tenant {ProducerTenant} exists", producerTenant);
                if (!ProducerTenantExists(producerTenant))
                {
                    var message = $"There are no consents configured for Producer Tenant {producerTenant}";
                    _logger.Debug(message);
                    return new EventHorizonContracts.SubscriptionResponse { Failure = new ProtobufContracts.Failure { Id = SubscriptionFailures.MissingConsent.ToProtobuf(), Reason = message,  } };
                }

                if (!TryGetConsentFor(consumerMicroservice, consumerTenant, producerTenant, publicStream, partition, out var consentId))
                {
                    var message = $"There are no consent configured for Partition {partition} in Public Stream {publicStream} in Tenant {producerTenant} to Consumer Tenant {consumerTenant} in Microservice {consumerMicroservice}";
                    _logger.Debug(message);
                    return new EventHorizonContracts.SubscriptionResponse { Failure = new ProtobufContracts.Failure { Id = SubscriptionFailures.MissingConsent.ToProtobuf(), Reason = message } };
                }

                return new EventHorizonContracts.SubscriptionResponse { ConsentId = consentId.ToProtobuf() };
            }
            catch (Exception ex)
            {
                const string message = "Error ocurred while creating subscription response";
                _logger.Warning(ex, message);
                return new EventHorizonContracts.SubscriptionResponse { Failure = new ProtobufContracts.Failure { Id = FailureId.Other.ToProtobuf(), Reason = message } };
            }
        }

        bool TryGetConsentFor(Microservice consumerMicroservice, TenantId consumerTenant, TenantId producerTenant, StreamId publicStream, PartitionId partition, out ConsentId consentId)
        {
            consentId = null;
            _logger.Trace(
                "Checking consents configured for Partition: {Partition} in Public Stream {PublicStream} in Tenant {ProducerTenant} to Consumer Tenant {ConsumerTenant} in Microservice {ConsumerMicroservice}",
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
                _logger.Debug(
                    "There are no consent configured for partition '{Partition}' in public stream '{PublicStream}' in tenant '{ProducerTenant}' to consumer tenant '{ConsumerTenant}' in microservice '{ConsumerMicroservice}'",
                    partition,
                    publicStream,
                    producerTenant,
                    consumerTenant,
                    consumerMicroservice);
                return false;
            }

            if (consentsForSubscription.Length > 1)
            {
                _logger.Warning(
                    "There are multiple consents configured for Partition {Partition} in Public Stream {PublicStream} in Tenant {ProducerTenant} to Consumer Tenant {ConsumerTenant} in Microservice {ConsumerMicroservice}",
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
}
