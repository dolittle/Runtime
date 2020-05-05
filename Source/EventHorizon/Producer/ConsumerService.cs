// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.ApplicationModel;
using Dolittle.ApplicationModel.Configuration;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static Dolittle.Runtime.EventHorizon.Contracts.Consumer;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    [Singleton]
    public class ConsumerService : ConsumerBase, IDisposable
    {
        readonly Microservice _thisMicroservice;
        readonly IExecutionContextManager _executionContextManager;
        readonly EventHorizonConsentsConfiguration _eventHorizonConsents;
        readonly ITenants _tenants;
        readonly FactoryFor<IEventFetchers> _getEventFetchers;
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
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ConsumerService(
            BoundedContextConfiguration boundedContextConfiguration,
            IExecutionContextManager executionContextManager,
            EventHorizonConsentsConfiguration eventHorizonConsents,
            ITenants tenants,
            FactoryFor<IEventFetchers> getEventFetchers,
            ILogger<ConsumerService> logger)
        {
            _thisMicroservice = boundedContextConfiguration.BoundedContext;
            _executionContextManager = executionContextManager;
            _eventHorizonConsents = eventHorizonConsents;
            _tenants = tenants;
            _getEventFetchers = getEventFetchers;
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
        public override async Task Subscribe(Contracts.ConsumerSubscription subscription, IServerStreamWriter<Contracts.SubscriptionMessage> responseStream, ServerCallContext context)
        {
            _executionContextManager.CurrentFor(subscription.CallContext.ExecutionContext);
            var consumerMicroservice = _executionContextManager.Current.Microservice;
            var consumerTenant = _executionContextManager.Current.Tenant;
            var producerTenant = subscription.TenantId.To<TenantId>();
            var publicStream = subscription.StreamId.To<StreamId>();
            var partition = subscription.PartitionId.To<PartitionId>();
            var lastReceivedPosition = subscription.LastReceived; // -1 if not received any events

            _logger.Debug($"Incomming Event Horizon subscription from microservice '{consumerMicroservice}' and tenant '{consumerTenant}' to tenant '{producerTenant}' starting at position '{lastReceivedPosition}' in partition '{partition}' in stream '{publicStream}'");
            try
            {
                var subscriptionResponse = CreateSubscriptionResponse(consumerMicroservice, consumerTenant, producerTenant, publicStream, partition);
                await responseStream.WriteAsync(new Contracts.SubscriptionMessage { SubscriptionResponse = subscriptionResponse }).ConfigureAwait(false);
                if (subscriptionResponse.Failure != null)
                {
                    _logger.Debug($"Denied subscription from microservice '{consumerMicroservice}' and tenant '{consumerTenant}' to partition '{partition}' in stream '{publicStream}' for tenant '{producerTenant}'");
                    return;
                }

                _logger.Information($"Microservice '{consumerMicroservice}' and tenant '{consumerTenant}' successfully subscrbed to tenant '{producerTenant}' starting at position '{lastReceivedPosition}' in partition '{partition}' in stream '{publicStream}'");

                var publicStreamPosition = new StreamPosition((ulong)(lastReceivedPosition + 1));
                _executionContextManager.CurrentFor(
                    _thisMicroservice,
                    producerTenant,
                    _executionContextManager.Current.CorrelationId);
                var publicEvents = await _getEventFetchers().GetPublicEventsFetcherFor(new StreamDefinition(new PublicFilterDefinition(StreamId.EventLog, publicStream)), context.CancellationToken).ConfigureAwait(false);
                while (!context.CancellationToken.IsCancellationRequested
                    && !_disposed)
                {
                    try
                    {
                        var streamEvent = await publicEvents.FetchInPartition(partition, publicStreamPosition, context.CancellationToken).ConfigureAwait(false);
                        if (streamEvent == default)
                        {
                            await Task.Delay(1000).ConfigureAwait(false);
                            continue;
                        }

                        var eventHorizonEvent = new Contracts.EventHorizonEvent
                        {
                            Content = streamEvent.Event.Content,
                            CorrelationId = streamEvent.Event.ExecutionContext.CorrelationId.ToProtobuf(),
                            EventSourceId = streamEvent.Event.EventSource.ToProtobuf(),
                            Occurred = Timestamp.FromDateTimeOffset(streamEvent.Event.Occurred),
                            Type = new Artifacts.Contracts.Artifact
                                {
                                    Generation = streamEvent.Event.Type.Generation,
                                    Id = streamEvent.Event.Type.Id.ToProtobuf()
                                },
                            StreamSequenceNumber = streamEvent.Position
                        };
                        await responseStream.WriteAsync(new Contracts.SubscriptionMessage { Event = eventHorizonEvent }).ConfigureAwait(false);
                        publicStreamPosition = streamEvent.Position + 1;
                    }
                    catch (EventStoreUnavailable)
                    {
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    _logger.Error(ex, $"Error occurred in Event Horizon between consumer microservice '{consumerMicroservice}' and tenant '{consumerTenant}' and producer tenant '{producerTenant}'");
                }

                throw;
            }
            finally
            {
                _logger.Warning($"Disconnecting Event Horizon between consumer microservice '{consumerMicroservice}' and tenant '{consumerTenant}' and producer tenant '{producerTenant}'");
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

        Contracts.SubscriptionResponse CreateSubscriptionResponse(Microservice consumerMicroservice, TenantId consumerTenant, TenantId producerTenant, StreamId publicStream, PartitionId partition)
        {
            _logger.Trace($"Checking if producer tenant '{producerTenant}' exists.");
            if (!ProducerTenantExists(producerTenant))
            {
                var message = $"";
                _logger.Debug(message);
                return new Contracts.SubscriptionResponse { Failure = new Protobuf.Contracts.Failure { Reason = message } };
            }

            if (!HasConsentFor(consumerMicroservice, consumerTenant, producerTenant, publicStream, partition))
            {
                var message = $"There are no consent configured for partition '{partition}' in public stream '{publicStream}' in tenant '{producerTenant}' to consumer tenant '{consumerTenant}' in microservice '{consumerMicroservice}'";
                _logger.Debug(message);
                return new Contracts.SubscriptionResponse { Failure = new Protobuf.Contracts.Failure { Reason = message } };
            }

            return new Contracts.SubscriptionResponse();
        }

        bool HasConsentFor(Microservice consumerMicroservice, TenantId consumerTenant, TenantId producerTenant, StreamId publicStream, PartitionId partition)
        {
            _logger.Trace($"Checking consents configured for partition '{partition}' in public stream '{publicStream}' in tenant '{producerTenant}' to consumer tenant '{consumerTenant}' in microservice '{consumerMicroservice}'");

            var consentsForSubscription = _eventHorizonConsents
                                            .GetConsentConfigurationsFor(producerTenant)
                                            .Where(_ => _.Microservice == consumerMicroservice && _.Tenant == consumerTenant && _.Stream == publicStream && _.Partition == partition).ToArray();

            if (consentsForSubscription.Length == 0)
            {
                var message = $"There are no consent configured for partition '{partition}' in public stream '{publicStream}' in tenant '{producerTenant}' to consumer tenant '{consumerTenant}' in microservice '{consumerMicroservice}'";
                _logger.Debug(message);
                return false;
            }

            if (consentsForSubscription.Length > 1)
            {
                _logger.Warning($"There are multiple consents configured for partition '{partition}' in public stream '{publicStream}' in tenant '{producerTenant}' to consumer tenant '{consumerTenant}' in microservice '{consumerMicroservice}'");
            }

            return true;
        }

        bool ProducerTenantExists(TenantId producerTenant) =>
            _tenants.All.Contains(producerTenant);
    }
}
