// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Processing.Streams.Unpartitioned;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessors" />.
    /// </summary>
    [SingletonPerTenant]
    public class StreamProcessors : IStreamProcessors
    {
        readonly ConcurrentDictionary<StreamProcessorId, StreamProcessor> _streamProcessors;
        readonly IPartitionedStreamProcessorStates _partitionedStreamProcessorStates;
        readonly IUnpartitionedStreamProcessorStates _unpartitionedStreamProcessorStates;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerManager _loggerManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessors"/> class.
        /// </summary>
        /// <param name="partitionedStreamProcessorStates">The <see cref="IPartitionedStreamProcessorStates" />.</param>
        /// <param name="unpartitionedStreamProcessorStates">The <see cref="IUnpartitionedStreamProcessorStates" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessors(
            IPartitionedStreamProcessorStates partitionedStreamProcessorStates,
            IUnpartitionedStreamProcessorStates unpartitionedStreamProcessorStates,
            IExecutionContextManager executionContextManager,
            ILoggerManager loggerManager,
            ILogger<StreamProcessors> logger)
        {
            _streamProcessors = new ConcurrentDictionary<StreamProcessorId, StreamProcessor>();
            _partitionedStreamProcessorStates = partitionedStreamProcessorStates;
            _unpartitionedStreamProcessorStates = unpartitionedStreamProcessorStates;
            _executionContextManager = executionContextManager;
            _loggerManager = loggerManager;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<StreamProcessorRegistration> Register(
            StreamDefinition streamDefinition,
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            CancellationToken cancellationToken)
        {
            var tenant = _executionContextManager.Current.Tenant;
            var streamProcessorId = new StreamProcessorId(eventProcessor.Scope, eventProcessor.Identifier, streamDefinition.StreamId);
            try
            {
                if (!_streamProcessors.ContainsKey(streamProcessorId))
                {
                    _logger.Warning("Stream Processor with Id: '{streamProcessorId}' for Tenant: '{tenant}' already registered", streamProcessorId, tenant);
                    return new FailedStreamProcessorRegistration($"Stream Processor with Id: '{streamProcessorId}' already registered", tenant);
                }

                var streamProcessor = await CreateStreamProcessor(streamProcessorId, streamDefinition, tenant, eventProcessor, eventsFromStreamsFetcher, cancellationToken).ConfigureAwait(false);
                if (!_streamProcessors.TryAdd(streamProcessorId, streamProcessor))
                {
                    _logger.Warning("Stream Processor with Id: '{streamProcessorId}' for Tenant: '{tenant}' already registered", streamProcessorId, tenant);
                    return new FailedStreamProcessorRegistration($"Stream Processor with Id: '{streamProcessorId}' already registered", tenant);
                }

                _logger.Trace("Stream Processor with Id: '{streamProcessorId}' registered for Tenant: '{tenant}'", tenant);
                return new SuccessfulStreamProcessorRegistration(streamProcessor, tenant, () => Unregister(streamProcessorId));
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to register Stream Processor with Id: '{streamProcessorId}' for Tenant: '{tenant}'", streamProcessorId);
                return new FailedStreamProcessorRegistration($"Failed to register Stream Processor with Id: '{streamProcessorId}'. {ex.Message}", tenant);
            }
        }

        /// <inheritdoc/>
        public void Unregister(StreamProcessorId streamProcessorId)
        {
            if (_streamProcessors.TryRemove(streamProcessorId, out var streamProcessor))
            {
                _logger.Debug($"Removing and disposing of Stream Processor with Id: '{streamProcessorId}'");
                streamProcessor.Stop();
            }
        }

        async Task<StreamProcessor> CreateStreamProcessor(
            StreamProcessorId streamProcessorId,
            StreamDefinition streamDefinition,
            TenantId tenant,
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            CancellationToken cancellationToken)
        {
            if (streamDefinition.Partitioned)
            {
                return await CreatePartitionedStreamProcessor(streamProcessorId, streamDefinition.StreamId, tenant, eventProcessor, eventsFromStreamsFetcher, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await CreateUnpartitionedStreamProcessor(streamProcessorId, streamDefinition.StreamId, tenant, eventProcessor, eventsFromStreamsFetcher, cancellationToken).ConfigureAwait(false);
            }
        }

        async Task<Partitioned.StreamProcessor> CreatePartitionedStreamProcessor(
            StreamProcessorId streamProcessorId,
            StreamId sourceStreamId,
            TenantId tenant,
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            CancellationToken cancellationToken)
        {
            var isPersisted = await _partitionedStreamProcessorStates.HasFor(streamProcessorId, cancellationToken).ConfigureAwait(false);
            Partitioned.StreamProcessorState streamProcessorState;
            if (isPersisted)
            {
                streamProcessorState = await _partitionedStreamProcessorStates.GetFor(streamProcessorId, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                streamProcessorState = Partitioned.StreamProcessorState.New;
                await _partitionedStreamProcessorStates.Persist(streamProcessorId, streamProcessorState, cancellationToken).ConfigureAwait(false);
            }

            return new Partitioned.StreamProcessor(
                tenant,
                sourceStreamId,
                streamProcessorState,
                eventProcessor,
                _partitionedStreamProcessorStates,
                eventsFromStreamsFetcher,
                new FailingPartitions(_partitionedStreamProcessorStates, eventsFromStreamsFetcher, _loggerManager.CreateLogger<FailingPartitions>()),
                _loggerManager.CreateLogger<Partitioned.StreamProcessor>(),
                cancellationToken);
        }

        async Task<Unpartitioned.StreamProcessor> CreateUnpartitionedStreamProcessor(
            StreamProcessorId streamProcessorId,
            StreamId sourceStreamId,
            TenantId tenant,
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            CancellationToken cancellationToken)
        {
            var isPersisted = await _unpartitionedStreamProcessorStates.HasFor(streamProcessorId, cancellationToken).ConfigureAwait(false);
            Unpartitioned.StreamProcessorState streamProcessorState;
            if (isPersisted)
            {
                streamProcessorState = await _unpartitionedStreamProcessorStates.GetFor(streamProcessorId, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                streamProcessorState = Unpartitioned.StreamProcessorState.New;
                await _unpartitionedStreamProcessorStates.Persist(streamProcessorId, streamProcessorState, cancellationToken).ConfigureAwait(false);
            }

            return new Unpartitioned.StreamProcessor(
                tenant,
                sourceStreamId,
                streamProcessorState,
                eventProcessor,
                _unpartitionedStreamProcessorStates,
                eventsFromStreamsFetcher,
                _loggerManager.CreateLogger<Unpartitioned.StreamProcessor>(),
                cancellationToken);
        }
    }
}