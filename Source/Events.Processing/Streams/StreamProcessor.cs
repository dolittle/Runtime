// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a system for working with all the <see cref="AbstractScopedStreamProcessor" /> registered for <see cref="ITenants.All" />.
    /// </summary>
    public class StreamProcessor
    {
        readonly IDictionary<TenantId, AbstractScopedStreamProcessor> _streamProcessors = new Dictionary<TenantId, AbstractScopedStreamProcessor>();
        readonly StreamProcessorId _identifier;
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly StreamId _sourceStreamId;
        readonly IStreamDefinition _streamDefinition;
        readonly Func<IEventProcessor> _getEventProcessor;
        readonly Action _unregister;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
        readonly FactoryFor<IEventFetchers> _getEventFetchers;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitions;
        readonly ILoggerManager _loggerManager;
        readonly ILogger<StreamProcessor> _logger;
        readonly CancellationToken _cancellationToken;
        bool _initialized;
        bool _started;
        bool _finishedProcessing;

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
        /// <param name="getStreamDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitionRepository" />.</param>
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
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitions,
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
            _getStreamDefinitions = getStreamDefinitions;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<StreamProcessor>();
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="sourceStreamId">The source <see cref="StreamId" />.</param>
        /// <param name="getEventProcessor">The <see cref="Func{TResult}" /> that returns an <see cref="IEventProcessor" />.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="ScopedStreamProcessor" />.</param>
        /// <param name="getStreamProcessorStates">The <see cref="FactoryFor{T}" /> <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="getEventFetchers">The <see cref="FactoryFor{T}" /> <see cref="IEventFetchers" />.</param>
        /// <param name="getStreamDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitionRepository" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            StreamProcessorId streamProcessorId,
            IPerformActionOnAllTenants onAllTenants,
            StreamId sourceStreamId,
            Func<IEventProcessor> getEventProcessor,
            Action unregister,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
            FactoryFor<IEventFetchers> getEventFetchers,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitions,
            ILoggerManager loggerManager,
            CancellationToken cancellationToken)
        {
            _identifier = streamProcessorId;
            _onAllTenants = onAllTenants;
            _sourceStreamId = sourceStreamId;
            _getEventProcessor = getEventProcessor;
            _unregister = unregister;
            _getStreamProcessorStates = getStreamProcessorStates;
            _getEventFetchers = getEventFetchers;
            _getStreamDefinitions = getStreamDefinitions;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<StreamProcessor>();
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Initializes the stream processor.
        /// </summary>
        /// <returns>A <see cref="Task" />that represents the asynchronous operation.</returns>
        public async Task Initialize()
        {
            _cancellationToken.ThrowIfCancellationRequested();
            if (_initialized) throw new StreamProcessorAlreadyInitialized(_identifier);
            _initialized = true;
            try
            {
                await _onAllTenants.PerformAsync(async tenant =>
                {
                    var streamDefinition = _streamDefinition;
                    if (streamDefinition == default)
                    {
                        var tryGetStreamDefinition = await _getStreamDefinitions().TryGet(_identifier.ScopeId, _sourceStreamId, _cancellationToken).ConfigureAwait(false);
                        if (!tryGetStreamDefinition.Success) throw new CannotCreateStreamProcessorOnUndefinedStream(_identifier);
                        streamDefinition = tryGetStreamDefinition.Result;
                    }

                    var scopedStreamProcessor = await CreateScopedStreamProcessor(
                        tenant,
                        streamDefinition,
                        _getEventProcessor(),
                        await _getEventFetchers().GetFetcherFor(_identifier.ScopeId, _streamDefinition, _cancellationToken).ConfigureAwait(false),
                        _getStreamProcessorStates()).ConfigureAwait(false);
                    _streamProcessors.Add(tenant, scopedStreamProcessor);
                }).ConfigureAwait(false);
            }
            catch
            {
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
            try
            {
                var task = _streamProcessors.Count > 0 ?
                                Task.WhenAll(_streamProcessors.Select(_ => _.Value.Start()))
                                : Task.CompletedTask;
                await task.ConfigureAwait(false);
            }
            finally
            {
                _finishedProcessing = true;
                _unregister();
            }
        }

        /// <summary>
        /// Unregisters the <see cref="StreamProcessor" />.
        /// </summary>
        public void Unregister()
        {
            if (_started && !_finishedProcessing) throw new CannotUnregisterRunningStreamProcessor(_identifier);
            _unregister();
        }

        async Task<AbstractScopedStreamProcessor> CreateScopedStreamProcessor(
            TenantId tenant,
            IStreamDefinition streamDefinition,
            IEventProcessor eventProcessor,
            ICanFetchEventsFromStream eventsFromStreamsFetcher,
            IStreamProcessorStateRepository streamProcessorStates)
        {
            if (streamDefinition.Partitioned)
            {
                return await CreatePartitionedScopedStreamProcessor(tenant, streamDefinition.StreamId, eventProcessor, eventsFromStreamsFetcher, streamProcessorStates).ConfigureAwait(false);
            }
            else
            {
                return await CreateUnpartitionedScopedStreamProcessor(tenant, streamDefinition.StreamId, eventProcessor, eventsFromStreamsFetcher, streamProcessorStates).ConfigureAwait(false);
            }
        }

        async Task<Partitioned.ScopedStreamProcessor> CreatePartitionedScopedStreamProcessor(
            TenantId tenant,
            StreamId sourceStreamId,
            IEventProcessor eventProcessor,
            ICanFetchEventsFromStream eventsFromStreamsFetcher,
            IStreamProcessorStateRepository streamProcessorStates)
        {
            var tryGetStreamProcessorState = await streamProcessorStates.TryGetFor(_identifier, _cancellationToken).ConfigureAwait(false);
            if (!tryGetStreamProcessorState.Success)
            {
                tryGetStreamProcessorState = Partitioned.StreamProcessorState.New;
                await streamProcessorStates.Persist(_identifier, tryGetStreamProcessorState.Result, _cancellationToken).ConfigureAwait(false);
            }

            if (!tryGetStreamProcessorState.Result.Partitioned) throw new ExpectedPartitionedStreamProcessorState(_identifier);

            return new Partitioned.ScopedStreamProcessor(
                tenant,
                sourceStreamId,
                tryGetStreamProcessorState.Result as Partitioned.StreamProcessorState,
                eventProcessor,
                streamProcessorStates,
                eventsFromStreamsFetcher,
                new Partitioned.FailingPartitions(streamProcessorStates, eventProcessor, eventsFromStreamsFetcher, _loggerManager.CreateLogger<Partitioned.FailingPartitions>()),
                _loggerManager.CreateLogger<Partitioned.ScopedStreamProcessor>(),
                _cancellationToken);
        }

        async Task<ScopedStreamProcessor> CreateUnpartitionedScopedStreamProcessor(
            TenantId tenant,
            StreamId sourceStreamId,
            IEventProcessor eventProcessor,
            ICanFetchEventsFromStream eventsFromStreamsFetcher,
            IStreamProcessorStateRepository streamProcessorStates)
        {
            var tryGetStreamProcessorState = await streamProcessorStates.TryGetFor(_identifier, _cancellationToken).ConfigureAwait(false);
            if (!tryGetStreamProcessorState.Success)
            {
                tryGetStreamProcessorState = StreamProcessorState.New;
                await streamProcessorStates.Persist(_identifier, tryGetStreamProcessorState.Result, _cancellationToken).ConfigureAwait(false);
            }

            if (tryGetStreamProcessorState.Result.Partitioned) throw new ExpectedUnpartitionedStreamProcessorState(_identifier);
            return new ScopedStreamProcessor(
                tenant,
                sourceStreamId,
                tryGetStreamProcessorState.Result as StreamProcessorState,
                eventProcessor,
                streamProcessorStates,
                eventsFromStreamsFetcher,
                _loggerManager.CreateLogger<ScopedStreamProcessor>(),
                _cancellationToken);
        }
    }
}