// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.EventHorizon;
using Dolittle.Applications;
using Dolittle.Applications.Configuration;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static contracts::Dolittle.Runtime.EventHorizon.Consumer;
using grpcArtifacts = contracts::Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    [Singleton]
    public class ConsumerService : ConsumerBase
    {
        readonly Application _application;
        readonly Microservice _microservice;
        readonly IExecutionContextManager _executionContextManager;
        readonly IEventHorizonConsents _eventHorizonConsents;
        readonly ITenants _tenants;
        readonly FactoryFor<IFetchEventsFromPublicStreams> _getEventsFromPublicStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerService"/> class.
        /// </summary>
        /// <param name="boundedContextConfiguration">The <see cref="BoundedContextConfiguration" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="eventHorizonConsents">The <see cref="IEventHorizonConsents" />.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="getEventsFromPublicStreamsFetcher">The <see cref="FactoryFor{T}" /> <see cref="IFetchEventsFromPublicStreams" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ConsumerService(
            BoundedContextConfiguration boundedContextConfiguration,
            IExecutionContextManager executionContextManager,
            IEventHorizonConsents eventHorizonConsents,
            ITenants tenants,
            FactoryFor<IFetchEventsFromPublicStreams> getEventsFromPublicStreamsFetcher,
            ILogger logger)
        {
            _application = boundedContextConfiguration.Application;
            _microservice = boundedContextConfiguration.BoundedContext;
            _executionContextManager = executionContextManager;
            _eventHorizonConsents = eventHorizonConsents;
            _tenants = tenants;
            _getEventsFromPublicStreamsFetcher = getEventsFromPublicStreamsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Subscribe(ConsumerSubscription subscription, IServerStreamWriter<EventHorizonEvent> responseStream, ServerCallContext context)
        {
            EventHorizon eventHorizon = null;
            try
            {
                eventHorizon = new EventHorizon(
                    _executionContextManager.Current.Microservice,
                    _executionContextManager.Current.Tenant,
                    _microservice,
                    subscription.Tenant.To<TenantId>());
                var lastReceivedPosition = subscription.LastReceived; // -1 if not received any events
                _logger.Information($"Incomming Event Horizon subscription from microservice '{eventHorizon.ConsumerMicroservice}' and tenant '{eventHorizon.ConsumerTenant}' to tenant '{eventHorizon.ProducerTenant}' starting at position '{lastReceivedPosition}'");

                ThrowIfProducerTenantDoesNotExist(eventHorizon.ProducerTenant, eventHorizon.ConsumerMicroservice, eventHorizon.ConsumerTenant);
                var publicStream = subscription.Stream.To<StreamId>();
                var partition = subscription.Partition.To<PartitionId>();
                var publicStreamPosition = new StreamPosition((ulong)(lastReceivedPosition + 1));

                ThrowIfConsentIsNotGiven(eventHorizon, publicStream, partition);
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    _executionContextManager.CurrentFor(
                        _application,
                        eventHorizon.ProducerMicroservice,
                        eventHorizon.ProducerTenant,
                        _executionContextManager.Current.CorrelationId);
                    try
                    {
                        var streamPosition = await _getEventsFromPublicStreamsFetcher().FindNext(publicStream, partition, publicStreamPosition, context.CancellationToken).ConfigureAwait(false);
                        if (streamPosition == uint.MaxValue) throw new NoEventInStreamAtPosition(ScopeId.Default, publicStream, publicStreamPosition);
                        var streamEvent = await _getEventsFromPublicStreamsFetcher().Fetch(publicStream, streamPosition, context.CancellationToken).ConfigureAwait(false);
                        var eventHorizonEvent = new EventHorizonEvent
                        {
                            Content = streamEvent.Event.Content,
                            Correlation = streamEvent.Event.CorrelationId.ToProtobuf(),
                            EventSource = streamEvent.Event.EventSource.ToProtobuf(),
                            Occurred = Timestamp.FromDateTimeOffset(streamEvent.Event.Occurred),
                            Type = new grpcArtifacts.Artifact
                                {
                                    Generation = streamEvent.Event.Type.Generation,
                                    Id = streamEvent.Event.Type.Id.ToProtobuf()
                                }
                        };
                        await responseStream.WriteAsync(eventHorizonEvent).ConfigureAwait(false);
                        publicStreamPosition = streamPosition.Increment();
                    }
                    catch (NoEventInStreamAtPosition)
                    {
                        await Task.Delay(1000).ConfigureAwait(false);
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
                    _logger.Error(ex, $"Error occurred in Event Horizon between consumer microservice '{eventHorizon.ConsumerMicroservice}' and tenant '{eventHorizon.ConsumerTenant}' and producer tenant '{eventHorizon.ProducerTenant}'");
                }

                throw;
            }
            finally
            {
                _logger.Warning($"Disconnecting Event Horizon between consumer microservice '{eventHorizon.ConsumerMicroservice}' and tenant '{eventHorizon.ConsumerTenant}' and producer tenant '{eventHorizon.ProducerTenant}'");
            }
        }

        void ThrowIfConsentIsNotGiven(EventHorizon eventHorizon, StreamId publicStream, PartitionId partition)
        {
            _eventHorizonConsents.GetConsentFor(eventHorizon.ProducerTenant, eventHorizon.ConsumerMicroservice, eventHorizon.ConsumerTenant, publicStream, partition);
        }

        void ThrowIfProducerTenantDoesNotExist(TenantId producerTenant, Microservice consumerMicroservice, TenantId consumerTenant)
        {
            if (!_tenants.All.Contains(producerTenant)) throw new ProducerTenantDoesNotExist(producerTenant, consumerMicroservice, consumerTenant);
        }
    }
}
