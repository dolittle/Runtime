// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Applications;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Processing.EventHorizon;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    [Singleton]
    public class EventHorizonService : EventHorizonBase
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly FactoryFor<IFetchEventsFromStreams> _getEventsFromStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonService"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="getEventsFromStreamsFetcher">The <see cref="FactoryFor{IFetchEventsFromStreams}" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHorizonService(
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            FactoryFor<IFetchEventsFromStreams> getEventsFromStreamsFetcher,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _getEventsFromStreamsFetcher = getEventsFromStreamsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Subscribe(EventHorizonSubscriberToPublisherRequest subscription, IServerStreamWriter<EventHorizonPublisherToSubscriberResponse> responseStream, ServerCallContext context)
        {
            Microservice microservice = null;
            TenantId subscriber = null;
            TenantId producer = null;
            try
            {
                microservice = subscription.Microservice.To<Microservice>();
                subscriber = subscription.SubscriberTenant.To<TenantId>();
                producer = subscription.ProducerTenant.To<TenantId>();
                var publicEventsVersion = subscription.PublicEventsVersion;
                _logger.Information($"Inncomming Event Horizon subscription from microservice '{microservice}' and tenant '{subscriber}' to tenant '{producer}' starting at version '{publicEventsVersion}'");

                if (!_tenants.All.Contains(producer)) throw new ProducerTenantDoesNotExist(producer, microservice);

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        _executionContextManager.CurrentFor(producer);
                        var publicEvent = await _getEventsFromStreamsFetcher().Fetch(StreamId.PublicEventsId, publicEventsVersion, context.CancellationToken).ConfigureAwait(false);
                        var convertedEvent = ConvertPublicEvent(publicEvent, subscriber);
                        var response = new EventHorizonPublisherToSubscriberResponse
                        {
                            Event = convertedEvent.ToProtobuf()
                        };
                        await responseStream.WriteAsync(response).ConfigureAwait(false);
                        publicEventsVersion++;
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
                    _logger.Error(ex, $"Error occurred while handling event horizon from microservice '{microservice}'");
                }
            }
            finally
            {
                _logger.Information($"Event horizons for microservice from microservice '{microservice}' and tenant '{subscriber}' to tenant '{producer}'");
            }
        }

        CommittedEvent ConvertPublicEvent(StreamEvent publicEvent, TenantId subscriber) =>
            new CommittedEvent(
                publicEvent.Event.EventLogVersion,
                publicEvent.Event.Occurred,
                publicEvent.Event.EventSource,
                publicEvent.Event.CorrelationId,
                publicEvent.Event.Microservice,
                subscriber,
                publicEvent.Event.Cause,
                publicEvent.Event.Type,
                true,
                publicEvent.Event.Content);
    }
}