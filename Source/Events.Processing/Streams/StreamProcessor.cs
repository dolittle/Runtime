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
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
        readonly FactoryFor<IEventFetchers> _getEventFetchers;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerManager _loggerManager;
        readonly ILogger<StreamProcessor> _logger;
        readonly CancellationToken _externalCancellationToken;
        readonly CancellationTokenSource _internalCancellationTokenSource;
        CancellationTokenRegistration _initializeCancellationTokenRegistration;
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
        /// <param name="getStreamProcessorStates">The <see cref="FactoryFor{T}" /> <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="getEventFetchers">The <see cref="FactoryFor{T}" /> <see cref="IEventFetchers" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            StreamProcessorId streamProcessorId,
            IPerformActionOnAllTenants onAllTenants,
            IStreamDefinition streamDefinition,
            Func<IEventProcessor> getEventProcessor,
            Action unregister,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
            FactoryFor<IEventFetchers> getEventFetchers,
            IExecutionContextManager executionContextManager,
            ILoggerManager loggerManager,
            CancellationToken cancellationToken)
        {
            _identifier = streamProcessorId;
            _onAllTenants = onAllTenants;
            _streamDefinition = streamDefinition;
            _getEventProcessor = getEventProcessor;
            _unregister = unregister;
            _getStreamProcessorStates = getStreamProcessorStates;
            _getEventFetchers = getEventFetchers;
            _executionContextManager = executionContextManager;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<StreamProcessor>();
            _internalCancellationTokenSource = new CancellationTokenSource();
            _externalCancellationToken = cancellationToken;
        }

        /// <summary>
        /// Initializes the stream processor.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" />that represents the asynchronous operation.</returns>
        public async Task Initialize(CancellationToken cancellationToken)
        {
            _externalCancellationToken.ThrowIfCancellationRequested();
            if (_initialized) throw new StreamProcessorAlreadyInitialized(_identifier);
            _initializeCancellationTokenRegistration = cancellationToken.Register(_unregister);
            try
            {
                await _onAllTenants.PerformAsync(async tenant =>
                {
                    var scopedStreamProcessor = await CreateScopedStreamProcessor(
                        tenant,
                        _getEventProcessor(),
                        _getEventFetchers(),
                        _getStreamProcessorStates()).ConfigureAwait(false);
                    _streamProcessors.Add(tenant, scopedStreamProcessor);
                }).ConfigureAwait(false);
                _initialized = true;
            }
            catch
            {
                _initializeCancellationTokenRegistration.Dispose();
                _unregister();
                throw;
            }
        }

        /// <summary>
        /// Starts the stream processing for all tenants.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Start()
        {
            if (!_initialized) throw new StreamProcessorNotInitialized(_identifier);
            if (_started) throw new StreamProcessorAlreadyProcessingStream(_identifier);
            _started = true;
            _initializeCancellationTokenRegistration.Dispose();
            try
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_internalCancellationTokenSource.Token, _externalCancellationToken);
                var tasks = StartScopedStreamProessors(linkedTokenSource.Token);
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
                _logger.Debug("Unregistering Stream Processor: {streamProcessorId}", _identifier);
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
            if (!_disposed)
            {
                if (disposing)
                {
                    if (!_internalCancellationTokenSource.IsCancellationRequested) _internalCancellationTokenSource.Cancel();
                    _internalCancellationTokenSource.Dispose();
                    _initializeCancellationTokenRegistration.Dispose();
                }

                if (!_internalCancellationTokenSource.IsCancellationRequested) _internalCancellationTokenSource.Cancel();
                if (!_started) _unregister();
                _disposed = true;
            }
        }

        IEnumerable<Task> StartScopedStreamProessors(CancellationToken cancellationToken)
        {
            var tasks = _streamProcessors.Select(_ => Task.Run(() =>
                    {
                        (var tenant, var streamProcessor) = _;
                        _executionContextManager.CurrentFor(tenant);
                        return streamProcessor.Start(cancellationToken);
                    }));
            return tasks = tasks.Any() ? tasks : new[] { Task.CompletedTask };
        }

        bool TryGetException(IEnumerable<Task> tasks, out Exception exception)
        {
            exception = tasks.FirstOrDefault(_ => _.Exception != default)?.Exception;
            if (exception != default)
            {
                while (exception.InnerException != null) exception = exception.InnerException;
            }

            return exception != default;
        }

        async Task<AbstractScopedStreamProcessor> CreateScopedStreamProcessor(
            TenantId tenant,
            IEventProcessor eventProcessor,
            IEventFetchers eventFetchers,
            IStreamProcessorStateRepository streamProcessorStates)
        {
            if (_streamDefinition.Partitioned)
            {
                var eventFetcher = await eventFetchers.GetPartitionedFetcherFor(eventProcessor.Scope, _streamDefinition, _externalCancellationToken).ConfigureAwait(false);
                return await CreatePartitionedScopedStreamProcessor(tenant, eventProcessor, eventFetcher, streamProcessorStates).ConfigureAwait(false);
            }
            else
            {
                var eventFetcher = await eventFetchers.GetFetcherFor(eventProcessor.Scope, _streamDefinition, _externalCancellationToken).ConfigureAwait(false);
                return await CreateUnpartitionedScopedStreamProcessor(tenant, eventProcessor, eventFetcher, streamProcessorStates).ConfigureAwait(false);
            }
        }

        async Task<Partitioned.ScopedStreamProcessor> CreatePartitionedScopedStreamProcessor(
            TenantId tenant,
            IEventProcessor eventProcessor,
            ICanFetchEventsFromPartitionedStream eventsFromStreamsFetcher,
            IStreamProcessorStateRepository streamProcessorStates)
        {
            var tryGetStreamProcessorState = await streamProcessorStates.TryGetFor(_identifier, _externalCancellationToken).ConfigureAwait(false);
            if (!tryGetStreamProcessorState.Success)
            {
                tryGetStreamProcessorState = Partitioned.StreamProcessorState.New;
                await streamProcessorStates.Persist(_identifier, tryGetStreamProcessorState.Result, _externalCancellationToken).ConfigureAwait(false);
            }

            if (!tryGetStreamProcessorState.Result.Partitioned) throw new ExpectedPartitionedStreamProcessorState(_identifier);

            return new Partitioned.ScopedStreamProcessor(
                tenant,
                _identifier,
                tryGetStreamProcessorState.Result as Partitioned.StreamProcessorState,
                eventProcessor,
                streamProcessorStates,
                eventsFromStreamsFetcher,
                new Partitioned.FailingPartitions(streamProcessorStates, eventProcessor, eventsFromStreamsFetcher, _loggerManager.CreateLogger<Partitioned.FailingPartitions>()),
                _loggerManager.CreateLogger<Partitioned.ScopedStreamProcessor>());
        }

        async Task<ScopedStreamProcessor> CreateUnpartitionedScopedStreamProcessor(
            TenantId tenant,
            IEventProcessor eventProcessor,
            ICanFetchEventsFromStream eventsFromStreamsFetcher,
            IStreamProcessorStateRepository streamProcessorStates)
        {
            var tryGetStreamProcessorState = await streamProcessorStates.TryGetFor(_identifier, _externalCancellationToken).ConfigureAwait(false);
            if (!tryGetStreamProcessorState.Success)
            {
                tryGetStreamProcessorState = StreamProcessorState.New;
                await streamProcessorStates.Persist(_identifier, tryGetStreamProcessorState.Result, _externalCancellationToken).ConfigureAwait(false);
            }

            if (tryGetStreamProcessorState.Result.Partitioned) throw new ExpectedUnpartitionedStreamProcessorState(_identifier);
            return new ScopedStreamProcessor(
                tenant,
                _identifier,
                tryGetStreamProcessorState.Result as StreamProcessorState,
                eventProcessor,
                streamProcessorStates,
                eventsFromStreamsFetcher,
                _loggerManager.CreateLogger<ScopedStreamProcessor>());
        }
    }
}