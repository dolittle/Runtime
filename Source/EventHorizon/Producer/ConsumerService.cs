// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading.Tasks;
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
using grpcEventHorizon = contracts::Dolittle.Runtime.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    [Singleton]
    public class ConsumerService : ConsumerBase
    {
        readonly Application _thisApplication;
        readonly Microservice _thisMicroservice;
        readonly IExecutionContextManager _executionContextManager;
        readonly EventHorizonConsentsConfiguration _eventHorizonConsents;
        readonly ITenants _tenants;
        readonly FactoryFor<IFetchEventsFromPublicStreams> _getEventsFromPublicStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerService"/> class.
        /// </summary>
        /// <param name="boundedContextConfiguration">The <see cref="BoundedContextConfiguration" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="eventHorizonConsents">The <see cref="EventHorizonConsentsConfiguration" />.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="getEventsFromPublicStreamsFetcher">The <see cref="FactoryFor{T}" /> <see cref="IFetchEventsFromPublicStreams" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ConsumerService(
            BoundedContextConfiguration boundedContextConfiguration,
            IExecutionContextManager executionContextManager,
            EventHorizonConsentsConfiguration eventHorizonConsents,
            ITenants tenants,
            FactoryFor<IFetchEventsFromPublicStreams> getEventsFromPublicStreamsFetcher,
            ILogger logger)
        {
            _thisApplication = boundedContextConfiguration.Application;
            _thisMicroservice = boundedContextConfiguration.BoundedContext;
            _executionContextManager = executionContextManager;
            _eventHorizonConsents = eventHorizonConsents;
            _tenants = tenants;
            _getEventsFromPublicStreamsFetcher = getEventsFromPublicStreamsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Subscribe(grpcEventHorizon.ConsumerSubscription subscription, IServerStreamWriter<grpcEventHorizon.SubscriptionStreamMessage> responseStream, ServerCallContext context)
        {
            var consumerMicroservice = _executionContextManager.Current.Microservice;
            var consumerTenant = _executionContextManager.Current.Tenant;
            var producerTenant = subscription.Tenant.To<TenantId>();
            var publicStream = subscription.Stream.To<StreamId>();
            var partition = subscription.Partition.To<PartitionId>();
            var lastReceivedPosition = subscription.LastReceived; // -1 if not received any events

            _logger.Debug($"Incomming Event Horizon subscription from microservice '{consumerMicroservice}' and tenant '{consumerTenant}' to tenant '{producerTenant}' starting at position '{lastReceivedPosition}' in partition '{partition}' in stream '{publicStream}'");
            try
            {
                var subscriptionResponse = CreateSubscriptionResponse(consumerMicroservice, consumerTenant, producerTenant, publicStream, partition);
                await responseStream.WriteAsync(new grpcEventHorizon.SubscriptionStreamMessage { SubscriptionResponse = subscriptionResponse }).ConfigureAwait(false);
                if (subscriptionResponse.Failure != null)
                {
                    _logger.Debug($"Denied subscription from microservice '{consumerMicroservice}' and tenant '{consumerTenant}' to partition '{partition}' in stream '{publicStream}' for tenant '{producerTenant}'");
                    return;
                }

                var publicStreamPosition = new StreamPosition((ulong)(lastReceivedPosition + 1));
                _executionContextManager.CurrentFor(
                    _thisApplication,
                    _thisMicroservice,
                    producerTenant,
                    _executionContextManager.Current.CorrelationId);
                var publicEvents = _getEventsFromPublicStreamsFetcher();
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // TODO: Surround with policy for event store.
                        var streamPosition = await publicEvents.FindNext(publicStream, partition, publicStreamPosition, context.CancellationToken).ConfigureAwait(false); // TODO: Replace with Fetch next in partition
                        if (streamPosition == uint.MaxValue) throw new NoEventInStreamAtPosition(ScopeId.Default, publicStream, publicStreamPosition);
                        var streamEvent = await publicEvents.Fetch(publicStream, streamPosition, context.CancellationToken).ConfigureAwait(false);
                        var eventHorizonEvent = new grpcEventHorizon.EventHorizonEvent
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
                        await responseStream.WriteAsync(new grpcEventHorizon.SubscriptionStreamMessage { Event = eventHorizonEvent }).ConfigureAwait(false);
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
                    _logger.Error(ex, $"Error occurred in Event Horizon between consumer microservice '{consumerMicroservice}' and tenant '{consumerTenant}' and producer tenant '{producerTenant}'");
                }

                throw;
            }
            finally
            {
                _logger.Warning($"Disconnecting Event Horizon between consumer microservice '{consumerMicroservice}' and tenant '{consumerTenant}' and producer tenant '{producerTenant}'");
            }
        }

        grpcEventHorizon.SubscriptionResponse CreateSubscriptionResponse(Microservice consumerMicroservice, TenantId consumerTenant, TenantId producerTenant, StreamId publicStream, PartitionId partition)
        {
            if (!ProducerTenantExists(producerTenant))
            {
                var message = $"";
                _logger.Debug(message);
                return new grpcEventHorizon.SubscriptionResponse { Failure = new grpcEventHorizon.Failure { Reason = message } };
            }

            var consentsForSubscription = _eventHorizonConsents
                                            .GetConsentConfigurationsFor(producerTenant)
                                            .Where(_ => _.Microservice == consumerTenant && _.Tenant == consumerTenant && _.Stream == publicStream && _.Partition == partition).ToArray();

            if (consentsForSubscription.Length == 0)
            {
                var message = $"There are no consent configured for partition '{partition}' in public stream '{publicStream}' in tenant '{producerTenant}' to consumer tenant '{consumerTenant}' in microservice '{consumerMicroservice}'";
                _logger.Debug(message);
                return new grpcEventHorizon.SubscriptionResponse { Failure = new grpcEventHorizon.Failure { Reason = message } };
            }

            if (consentsForSubscription.Length > 1)
            {
                _logger.Warning($"There are multiple consents configured for partition '{partition}' in public stream '{publicStream}' in tenant '{producerTenant}' to consumer tenant '{consumerTenant}' in microservice '{consumerMicroservice}'");
            }

            return new grpcEventHorizon.SubscriptionResponse();
        }

        bool ProducerTenantExists(TenantId producerTenant) =>
            _tenants.All.Contains(producerTenant);
    }
}
