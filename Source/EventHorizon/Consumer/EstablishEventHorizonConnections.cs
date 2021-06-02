// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services.Clients;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using System;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEstablishEventHorizonConnections" />.
    /// </summary>
    [SingletonPerTenant]
    public class EstablishEventHorizonConnections : IEstablishEventHorizonConnections
    {
        readonly ILogger _logger;
        readonly IClientManager _clientManager;
        readonly MicroservicesConfiguration _microservicesConfiguration;
        readonly IAsyncPolicyFor<ConsumerClient> _policy;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IReverseCallClients _reverseCallClients;
        readonly ILoggerFactory _loggerFactory;

        public EstablishEventHorizonConnections(
            IClientManager clientManager,
            MicroservicesConfiguration microservicesConfiguration,
            IAsyncPolicyFor<ConsumerClient> policy,
            IStreamProcessorStateRepository streamProcessorStates,
            IReverseCallClients reverseCallClients,
            ILoggerFactory loggerFactory,
            ILogger logger)
        {
            _clientManager = clientManager;
            _microservicesConfiguration = microservicesConfiguration;
            _policy = policy;
            _streamProcessorStates = streamProcessorStates;
            _reverseCallClients = reverseCallClients;
            _loggerFactory = loggerFactory;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Try<SubscriptionResponse>> TryEstablish(
            SubscriptionId subscription,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon,
            CancellationToken cancellationToken)
        {
            try
            {
                var producerMicroservice = subscription.ProducerMicroserviceId;
                if (!TryGetMicroserviceAddress(producerMicroservice, out var connectionAddress))
                {
                    _logger.NoMicroserviceConfigurationFor(producerMicroservice);
                    return SubscriptionResponse.Failed(new Failure(
                        SubscriptionFailures.MissingMicroserviceConfiguration,
                        $"No microservice configuration for producer mircoservice {producerMicroservice}"));
                }

                var connection = new EventHorizonConnection(
                    subscription,
                    connectionAddress,
                    _clientManager.Get<Contracts.Consumer.ConsumerClient>(connectionAddress.Host, connectionAddress.Port),
                    _policy,
                    _streamProcessorStates,
                    _reverseCallClients,
                    eventsFromEventHorizon,
                    _loggerFactory.CreateLogger<EventHorizonConnection>(),
                    cancellationToken);

                return await connection.InitiateAndKeepConnection().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        bool TryGetMicroserviceAddress(Microservice producerMicroservice, out MicroserviceAddress microserviceAddress)
        {
            var result = _microservicesConfiguration.TryGetValue(producerMicroservice, out var microserviceAddressConfig);
            microserviceAddress = microserviceAddressConfig;
            return result;
        }
    }
}
