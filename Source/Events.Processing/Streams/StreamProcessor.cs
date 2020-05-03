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
        readonly IStreamDefinition _streamDefinition;
        readonly IEventProcessor _eventProcessor;
        readonly Action _unregister;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
        readonly FactoryFor<IEventFetchers> _getEventFetchers;
        readonly ILoggerManager _loggerManager;
        readonly ILogger<StreamProcessor> _logger;
        readonly CancellationToken _cancellationToken;
        bool _initialized;
        bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="eventProcessor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="unregister">An <see cref="Action" /> that unregisters the <see cref="ScopedStreamProcessor" />.</param>
        /// <param name="getStreamProcessorStates">The <see cref="FactoryFor{T}" /> <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="getEventFetchers">The <see cref="FactoryFor{T}" /> <see cref="IEventFetchers" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        public StreamProcessor(
            StreamProcessorId streamProcessorId,
            IPerformActionOnAllTenants onAllTenants,
            IStreamDefinition streamDefinition,
            IEventProcessor eventProcessor,
            Action unregister,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
            FactoryFor<IEventFetchers> getEventFetchers,
            ILoggerManager loggerManager,
            CancellationToken cancellationToken)
        {
            _identifier = streamProcessorId;
            _onAllTenants = onAllTenants;
            _streamDefinition = streamDefinition;
            _eventProcessor = eventProcessor;
            _unregister = unregister;
            _getStreamProcessorStates = getStreamProcessorStates;
            _getEventFetchers = getEventFetchers;
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
                    var scopedStreamProcessor = await CreateScopedStreamProcessor(
                        tenant,
                        await _getEventFetchers().GetFetcherFor(_streamDefinition, _cancellationToken).ConfigureAwait(false),
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
                _unregister();
            }
        }

        async Task<AbstractScopedStreamProcessor> CreateScopedStreamProcessor(
            TenantId tenant,
            ICanFetchEventsFromStream eventsFromStreamsFetcher,
            IStreamProcessorStateRepository streamProcessorStates)
        {
            if (_streamDefinition.Partitioned)
            {
                return await CreatePartitionedScopedStreamProcessor(tenant, eventsFromStreamsFetcher, streamProcessorStates).ConfigureAwait(false);
            }
            else
            {
                return await CreateUnpartitionedScopedStreamProcessor(tenant, eventsFromStreamsFetcher, streamProcessorStates).ConfigureAwait(false);
            }
        }

        async Task<Partitioned.ScopedStreamProcessor> CreatePartitionedScopedStreamProcessor(
            TenantId tenant,
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
                _streamDefinition.StreamId,
                tryGetStreamProcessorState.Result as Partitioned.StreamProcessorState,
                _eventProcessor,
                streamProcessorStates,
                eventsFromStreamsFetcher,
                new Partitioned.FailingPartitions(streamProcessorStates, _eventProcessor, eventsFromStreamsFetcher, _loggerManager.CreateLogger<Partitioned.FailingPartitions>()),
                _loggerManager.CreateLogger<Partitioned.ScopedStreamProcessor>(),
                _cancellationToken);
        }

        async Task<ScopedStreamProcessor> CreateUnpartitionedScopedStreamProcessor(
            TenantId tenant,
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
                _streamDefinition.StreamId,
                tryGetStreamProcessorState.Result as StreamProcessorState,
                _eventProcessor,
                streamProcessorStates,
                eventsFromStreamsFetcher,
                _loggerManager.CreateLogger<ScopedStreamProcessor>(),
                _cancellationToken);
        }
    }
}