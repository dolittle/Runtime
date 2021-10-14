// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Tenancy;

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
        readonly FactoryFor<IEventProcessor> _getEventProcessor;
        readonly Action _unregister;
        readonly FactoryFor<ICreateScopedStreamProcessors> _getScopedStreamProcessorsCreator;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger<StreamProcessor> _logger;
        readonly CancellationTokenSource _stopAllScopedStreamProcessorsTokenSource;
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
            FactoryFor<IEventProcessor> getEventProcessor,
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
            _stopAllScopedStreamProcessorsTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        /// <summary>
        /// Initializes the stream processor.
        /// </summary>
        /// <returns>A <see cref="Task" />that represents the asynchronous operation.</returns>
        public async Task Initialize()
        {
            _logger.InitializingStreamProcessor(_identifier);

            _stopAllScopedStreamProcessorsTokenSource.Token.ThrowIfCancellationRequested();
            if (_initialized)
            {
                throw new StreamProcessorAlreadyInitialized(_identifier);
            }
            _initialized = true;

            await _onAllTenants.PerformAsync(async tenant =>
                {
                    var scopedStreamProcessorsCreators = _getScopedStreamProcessorsCreator();
                    var scopedStreamProcessor = await scopedStreamProcessorsCreators.Create(
                        _streamDefinition,
                        _identifier,
                        _getEventProcessor(),
                        _stopAllScopedStreamProcessorsTokenSource.Token).ConfigureAwait(false);
                    _streamProcessors.Add(tenant, scopedStreamProcessor);
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts the stream processing for all tenants.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Start()
        {
            _logger.StartingStreamProcessor(_identifier);

            if (!_initialized)
            {
                throw new StreamProcessorNotInitialized(_identifier);
            }

            if (_started)
            {
                throw new StreamProcessorAlreadyProcessingStream(_identifier);
            }

            _started = true;
            try
            {
                var tasks = StartScopedStreamProcessors(_stopAllScopedStreamProcessorsTokenSource.Token);
                await Task.WhenAny(tasks).ConfigureAwait(false);
                if (TryGetException(tasks, out var ex))
                {
                    _logger.ScopedStreamProcessorFailed(ex, _identifier);
                }

                _stopAllScopedStreamProcessorsTokenSource.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            finally
            {
                _unregister();
            }
        }

        /// <summary>
        /// Sets the position of the stream processor for a tenant.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId"/>.</param>
        /// <param name="position">The <see cref="StreamPosition" />.</param>
        /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
        public Task<Try<StreamPosition>> SetToPosition(TenantId tenant, StreamPosition position)
            => _streamProcessors.TryGetValue(tenant, out var streamProcessor)
                ? streamProcessor.SetToPosition(position)
                : Task.FromResult<Try<StreamPosition>>(new StreamProcessorNotRegisteredForTenant(_identifier, tenant));

        /// <summary>
        /// Sets the position of the stream processors for all tenant to be the initial <see cref="StreamPosition"/>.
        /// </summary>
        /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
        public async Task<IDictionary<TenantId, Try<StreamPosition>>> SetToInitialPositionForAllTenants()
        {
            var tasks = _streamProcessors
                .ToDictionary(_ => _.Key, _ => _.Value.SetToPosition(StreamPosition.Start));

            var result = new Dictionary<TenantId, Try<StreamPosition>>();

            foreach (var (tenant, task) in tasks)
            {
                result.Add(tenant, await task.ConfigureAwait(false));
            }

            return result;
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

            if (disposing)
            {
                _stopAllScopedStreamProcessorsTokenSource.Cancel();
                _stopAllScopedStreamProcessorsTokenSource.Dispose();

                if (!_started)
                {
                    _unregister();
                }
            }

            _disposed = true;
        }

        IEnumerable<Task> StartScopedStreamProcessors(CancellationToken cancellationToken) => _streamProcessors.Select(
            _ => Task.Run(async () =>
                {
                    var (tenant, streamProcessor) = _;
                    _executionContextManager.CurrentFor(tenant);
                    await streamProcessor.Start(cancellationToken).ConfigureAwait(false);
                }, cancellationToken)).ToList();

        static bool TryGetException(IEnumerable<Task> tasks, out Exception exception)
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
