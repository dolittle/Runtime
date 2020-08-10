// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a system for working with all the <see cref="AbstractScopedStreamProcessor" /> registered for <see cref="ITenants.All" />.
    /// </summary>
    public class StreamProcessor : IDisposable
    {
        readonly IDictionary<TenantId, AbstractScopedStreamProcessor> _streamProcessors = new Dictionary<TenantId, AbstractScopedStreamProcessor>();
        readonly StreamProcessorId _identifier;
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly IStreamDefinition _streamDefinition;
        readonly Func<IEventProcessor> _getEventProcessor;
        readonly Action _unregister;
        readonly FactoryFor<ICreateScopedStreamProcessors> _getScopedStreamProcessorsCreator;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger<StreamProcessor> _logger;
        readonly CancellationToken _externalCancellationToken;
        readonly CancellationTokenSource _internalCancellationTokenSource;
        readonly CancellationTokenRegistration _unregisterTokenRegistration;
        bool _initialized;
        bool _started;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="getEventProcessor">The <see cref="Func{TResult}" /> that returns an <see cref="IEventProcessor" />.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="ScopedStreamProcessor" />.</param>
        /// <param name="getScopedStreamProcessorsCreator">The <see cref="ICreateScopedStreamProcessors" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            StreamProcessorId streamProcessorId,
            IPerformActionOnAllTenants onAllTenants,
            IStreamDefinition streamDefinition,
            Func<IEventProcessor> getEventProcessor,
            Action unregister,
            FactoryFor<ICreateScopedStreamProcessors> getScopedStreamProcessorsCreator,
            IExecutionContextManager executionContextManager,
            ILogger<StreamProcessor> logger,
            CancellationToken cancellationToken)
        {
            _identifier = streamProcessorId;
            _onAllTenants = onAllTenants;
            _streamDefinition = streamDefinition;
            _getEventProcessor = getEventProcessor;
            _unregister = unregister;
            _getScopedStreamProcessorsCreator = getScopedStreamProcessorsCreator;
            _executionContextManager = executionContextManager;
            _logger = logger;
            _internalCancellationTokenSource = new CancellationTokenSource();
            _externalCancellationToken = cancellationToken;
            _unregisterTokenRegistration = _externalCancellationToken.Register(_unregister);
        }

        /// <summary>
        /// Initializes the stream processor.
        /// </summary>
        /// <returns>A <see cref="Task" />that represents the asynchronous operation.</returns>
        public async Task Initialize()
        {
            _logger.Debug("Initializing StreamProcessor with Id: {StreamProcessorId}", _identifier);
            _externalCancellationToken.ThrowIfCancellationRequested();
            if (_initialized) throw new StreamProcessorAlreadyInitialized(_identifier);
            await _onAllTenants.PerformAsync(async tenant =>
                {
                    var scopedStreamProcessorsCreators = _getScopedStreamProcessorsCreator();
                    var scopedStreamProcessor = await scopedStreamProcessorsCreators.Create(
                        _streamDefinition,
                        _identifier,
                        _getEventProcessor(),
                        _externalCancellationToken).ConfigureAwait(false);
                    _streamProcessors.Add(tenant, scopedStreamProcessor);
                }).ConfigureAwait(false);
            _initialized = true;
        }

        /// <summary>
        /// Starts the stream processing for all tenants.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Start()
        {
            _logger.Debug("Starting StreamProcessor with Id: {StreamProcessorId}", _identifier);
            if (!_initialized) throw new StreamProcessorNotInitialized(_identifier);
            if (_started) throw new StreamProcessorAlreadyProcessingStream(_identifier);
            _started = true;
            _unregisterTokenRegistration.Dispose();
            try
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_internalCancellationTokenSource.Token, _externalCancellationToken);
                var tasks = StartScopedStreamProcessors(linkedTokenSource.Token);
                await Task.WhenAny(tasks).ConfigureAwait(false);
                if (TryGetException(tasks, out var ex))
                {
                    _logger.Warning(ex, "Scoped Stream Processor with Id: {streamProcessorId} failed", _identifier);
                }

                _internalCancellationTokenSource.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
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
        /// Dispose the object.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (!_internalCancellationTokenSource.IsCancellationRequested) _internalCancellationTokenSource.Cancel();
            if (!_started && !_externalCancellationToken.IsCancellationRequested) _unregister();
            if (disposing)
            {
                _internalCancellationTokenSource.Dispose();
                _unregisterTokenRegistration.Dispose();
            }

            _disposed = true;
        }

        IEnumerable<Task> StartScopedStreamProcessors(CancellationToken cancellationToken) => _streamProcessors.Select(
            _ => Task.Run(async () =>
                {
                    (var tenant, var streamProcessor) = _;
                    _executionContextManager.CurrentFor(tenant);
                    await streamProcessor.Start(cancellationToken).ConfigureAwait(false);
                })).ToList();

        bool TryGetException(IEnumerable<Task> tasks, out Exception exception)
        {
            exception = tasks.FirstOrDefault(_ => _.Exception != default)?.Exception;
            if (exception != default)
            {
                while (exception.InnerException != null) exception = exception.InnerException;
            }

            return exception != default;
        }
    }
}