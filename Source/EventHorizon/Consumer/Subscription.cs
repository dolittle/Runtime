// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using EventHorizonStreamProcessor = Dolittle.Runtime.EventHorizon.Consumer.Processing.IStreamProcessor;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscription" />.
    /// </summary>
    public class Subscription : ISubscription
    {
        readonly IEstablishEventHorizonConnections _connectionEstablisher;
        readonly ILogger _logger;
        readonly AsyncProducerConsumerQueue<StreamEvent> _eventFromEventHorizon;
        readonly CancellationToken _token;
        readonly CancellationTokenSource _cts = new();
        readonly MicroserviceAddress _connectionAddress;
        readonly IStreamProcessorFactory _streamProcessorFactory;
        IEventHorizonConnection _eventHorizonConnection;
        EventHorizonStreamProcessor _streamProcessor;
        bool _disposed;

        public Subscription(
            SubscriptionId identifier,
            MicroserviceAddress connectionAddress,
            IStreamProcessorFactory streamProcessorFactory,
            IEstablishEventHorizonConnections connectionEstablisher,
            ILoggerFactory loggerFactory)
        {
            Identifier = identifier;
            _connectionEstablisher = connectionEstablisher;
            _connectionAddress = connectionAddress;
            _streamProcessorFactory = streamProcessorFactory;
            _eventFromEventHorizon = new AsyncProducerConsumerQueue<StreamEvent>();
            _token = _cts.Token;
            _logger = loggerFactory.CreateLogger<Subscription>();
        }

        /// <summary>
        /// Gets the <see cref="ConsentId" />.
        /// </summary>
        public ConsentId Consent { get; private set; }

        /// <summary>
        /// Gets whether the Subscription has finished subscribing.
        /// </summary>
        public bool HasFinishedSubscribing => Consent != null;

        /// <summary>
        /// Gets the <see cref="SubscriptionId" />.
        /// </summary>
        public SubscriptionId Identifier { get; }

        /// <inheritdoc/>
        public async Task<SubscriptionResponse> Register()
        {
            _logger.LogDebug("Registering Subscription: {Identifier}", Identifier);
            _eventHorizonConnection = _connectionEstablisher.Establish(Identifier, _connectionAddress, _eventFromEventHorizon);
            var response = await _eventHorizonConnection.FirstSubscriptionResponse.ConfigureAwait(false);
            if (response.Success)
            {
                _logger.SuccessfullyRegisteredSubscription(Identifier);
                Consent = response.ConsentId;
                _streamProcessor = _streamProcessorFactory.Create(Consent, Identifier, new EventsFromEventHorizonFetcher(_eventFromEventHorizon));
                _ = Task.Run(StartStreamProcessor);
            }
            else
            {
                _logger.LogDebug("EventHorizonConnections first response failed for subscription: {Identifier}, disposing the connection", Identifier);
                _eventHorizonConnection.Dispose();
            }
            return response;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _cts.Cancel();
                _cts.Dispose();
                _eventHorizonConnection.Dispose();
            }

            _disposed = true;
        }

        async Task StartStreamProcessor()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    var tryStart = await _streamProcessor.TryStartAndWait(_token);
                    if (!_token.IsCancellationRequested)
                    {
                        if (tryStart.HasException)
                        {
                            _logger.LogWarning(tryStart.Exception, "An error occurred while starting stream processor in subscription {Subscription}", Identifier);
                        }
                        else
                        {
                            _logger.LogDebug("Stream processor for subscription {Subscription} stopped for some reason. Restarting", Identifier);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Something wrong happened while running stream processor in subscription {Subscription", Identifier);
                }

            }
            _logger.LogDebug("Subscription {Subscription} is being cancelled. Stream processor has been shutdown", Identifier);
        }
    }
}
