// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
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
using Dolittle.Security;
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
        readonly ITenants _tenants;
        readonly FactoryFor<IFetchEventsFromStreams> _getEventsFromStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerService"/> class.
        /// </summary>
        /// <param name="boundedContextConfiguration">The <see cref="BoundedContextConfiguration" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="getEventsFromStreamsFetcher">The <see cref="FactoryFor{IFetchEventsFromStreams}" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ConsumerService(
            BoundedContextConfiguration boundedContextConfiguration,
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            FactoryFor<IFetchEventsFromStreams> getEventsFromStreamsFetcher,
            ILogger logger)
        {
            _application = boundedContextConfiguration.Application;
            _microservice = boundedContextConfiguration.BoundedContext.Value;
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _getEventsFromStreamsFetcher = getEventsFromStreamsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Subscribe(ConsumerSubscription subscription, IServerStreamWriter<PublicEvent> responseStream, ServerCallContext context)
        {
            Microservice consumerMicroservice = null;
            TenantId subscriber = null;
            TenantId producer = null;
            try
            {
                consumerMicroservice = _executionContextManager.Current.BoundedContext.Value;
                subscriber = _executionContextManager.Current.Tenant;
                producer = subscription.Tenant.To<TenantId>();
                var publicEventsPosition = subscription.PublicEventsPosition;
                _logger.Information($"Incomming Event Horizon subscription from microservice '{consumerMicroservice}' and tenant '{subscriber}' to tenant '{producer}' starting at position '{publicEventsPosition}'");

                if (!_tenants.All.Contains(producer)) throw new ProducerTenantDoesNotExist(producer, consumerMicroservice);

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var environment = _executionContextManager.Current.Environment;
                        var culture = _executionContextManager.Current.Culture;
                        _executionContextManager.CurrentFor(new ExecutionContext(
                            _application,
                            _microservice.Value,
                            producer,
                            environment,
                            CorrelationId.New(),
                            Claims.Empty,
                            culture));
                        var streamEvent = await _getEventsFromStreamsFetcher().Fetch(StreamId.PublicEventsId, publicEventsPosition, context.CancellationToken).ConfigureAwait(false);
                        var publicEvent = new PublicEvent
                        {
                            ConsumerMicroservice = consumerMicroservice.ToProtobuf(),
                            ConsumerTenant = subscriber.ToProtobuf(),
                            Content = streamEvent.Event.Content,
                            Correlation = streamEvent.Event.CorrelationId.ToProtobuf(),
                            EventLogSequenceNumber = streamEvent.Event.EventLogSequenceNumber,
                            EventSource = streamEvent.Event.EventSource.ToProtobuf(),
                            Occurred = Timestamp.FromDateTimeOffset(streamEvent.Event.Occurred),
                            ProducerMicroservice = _microservice.ToProtobuf(),
                            ProducerTenant = producer.ToProtobuf(),
                            Type = new grpcArtifacts.Artifact
                                {
                                    Generation = streamEvent.Event.Type.Generation,
                                    Id = streamEvent.Event.Type.Id.ToProtobuf()
                                }
                        };
                        await responseStream.WriteAsync(publicEvent).ConfigureAwait(false);
                        publicEventsPosition++;
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
                    _logger.Error(ex, $"Error occurred while handling Event Horizon to microservice '{consumerMicroservice}' and tenant '{subscriber}' from tenant '{producer}'");
                }
            }
            finally
            {
                _logger.Warning($"Disconnecting Event Horizon to microservice '{consumerMicroservice}' and tenant '{subscriber}' from tenant '{producer}'");
            }
        }
    }
}
