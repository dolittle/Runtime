// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using EventHorizonStreamProcessor = Dolittle.Runtime.EventHorizon.Consumer.Processing.StreamProcessor;
using Nito.AsyncEx;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents a system for working with <see cref="ScopedStreamProcessor" /> registered for an Event Horizon Subscription.
    /// </summary>
    public class Subscription : IDisposable
    {
        readonly SubscriptionId _identifier;
        readonly IEstablishEventHorizonConnections _connectionEstablisher;
        readonly ILogger _logger;
        readonly EventHorizonStreamProcessor _streamProcessor;
        readonly AsyncProducerConsumerQueue<StreamEvent> _eventFromEventHorizon;
        readonly CancellationToken _externalCancellationToken;
        readonly CancellationTokenSource _cts;
        bool _registered;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="consentId">The <see cref="ConsentId" />.</param>
        /// <param name="subscriptionId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="eventsFetcher">The <see cref="EventsFromEventHorizonFetcher" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IResilientStreamProcessorStateRepository" />.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="ScopedStreamProcessor" />.</param>
        /// <param name="eventsFetcherPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public Subscription(
            ConsentId consent,
            SubscriptionId identifier,
            IStreamProcessorFactory streamProcessorFactory,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            Consent = consent;
            Identifier = identifier;
            _logger = loggerFactory.CreateLogger<Subscription>();
            _streamProcessor = streamProcessorFactory.Create(Consent, Identifier, new EventsFromEventHorizonFetcher(_eventFromEventHorizon));
            _externalCancellationToken = cancellationToken;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(_externalCancellationToken);
        }

        /// <summary>
        /// Gets the <see cref="ConsentId" />.
        /// </summary>
        public ConsentId Consent { get; }

        /// <summary>
        /// Gets the <see cref="SubscriptionId" />.
        /// </summary>
        /// <value></value>
        public SubscriptionId Identifier { get; }

        /// <summary>
        /// Starts the stream processing for all tenants.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Try<bool>> Register()
        {
            try
            {
                if (_registered)
                {
                    return new SubscriptionAlreadyRegistered(Identifier, Consent);
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
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
            if (_registered)
            {
                _cts.Cancel();
            }
            if (disposing)
            {
                _cts.Dispose();
            }

            _disposed = true;
        }
    }
}
