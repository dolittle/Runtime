// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents a system for working with <see cref="ScopedStreamProcessor" /> registered for an Event Horizon Subscription.
    /// </summary>
    public class Subscription : IDisposable
    {
        readonly SubscriptionId _identifier;
        readonly IEventProcessor _eventProcessor;
        readonly Action _unregister;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly EventsFromEventHorizonFetcher _eventsFetcher;
        readonly ILoggerManager _loggerManager;
        readonly ILogger _logger;
        readonly CancellationToken _cancellationToken;
        readonly CancellationTokenRegistration _unregisterTokenRegistration;
        ScopedStreamProcessor _streamProcessor;
        bool _initialized;
        bool _started;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="subscriptionId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="eventsFetcher">The <see cref="EventsFromEventHorizonFetcher" />.</param>
        /// <param name="streamProcessorStates">The <see cref="FactoryFor{T}" /> <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="ScopedStreamProcessor" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public Subscription(
            SubscriptionId subscriptionId,
            EventProcessor eventProcessor,
            EventsFromEventHorizonFetcher eventsFetcher,
            IStreamProcessorStateRepository streamProcessorStates,
            Action unregister,
            ILoggerManager loggerManager,
            CancellationToken cancellationToken)
        {
            _identifier = subscriptionId;
            _eventProcessor = eventProcessor;
            _unregister = unregister;
            _streamProcessorStates = streamProcessorStates;
            _eventsFetcher = eventsFetcher;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<StreamProcessor>();
            _cancellationToken = cancellationToken;
            _unregisterTokenRegistration = _cancellationToken.Register(_unregister);
        }

        /// <summary>
        /// Initializes the stream processor.
        /// </summary>
        /// <returns>A <see cref="Task" />that represents the asynchronous operation.</returns>
        public async Task Initialize()
        {
            _cancellationToken.ThrowIfCancellationRequested();
            if (_initialized) throw new StreamProcessorAlreadyInitialized(_identifier);
            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(_identifier, _cancellationToken).ConfigureAwait(false);
            if (!tryGetStreamProcessorState.Success)
            {
                tryGetStreamProcessorState = StreamProcessorState.New;
                await _streamProcessorStates.Persist(_identifier, tryGetStreamProcessorState.Result, _cancellationToken).ConfigureAwait(false);
            }

            _streamProcessor = new ScopedStreamProcessor(
                _identifier.ConsumerTenantId,
                _identifier,
                tryGetStreamProcessorState.Result as StreamProcessorState,
                _eventProcessor,
                _streamProcessorStates,
                _eventsFetcher,
                _loggerManager.CreateLogger<ScopedStreamProcessor>());
            _initialized = true;
        }

        /// <summary>
        /// Starts the stream processing for all tenants.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Start()
        {
            if (!_initialized) throw new StreamProcessorNotInitialized(_identifier);
            if (_started) throw new StreamProcessorAlreadyProcessingStream(_identifier);
            _unregisterTokenRegistration.Dispose();
            _started = true;
            try
            {
                await _streamProcessor.Start(_cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _unregister();
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
            if (_disposed) return;
            if (!_started && !_cancellationToken.IsCancellationRequested) _unregister();
            if (disposing)
            {
                _unregisterTokenRegistration.Dispose();
            }

            _disposed = true;
        }
    }
}