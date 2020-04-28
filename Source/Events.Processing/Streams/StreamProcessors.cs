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
        readonly ConcurrentDictionary<StreamProcessorId, AbstractStreamProcessor> _streamProcessors;
        readonly IStreamProcessorStates _streamProcessorStates;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerManager _loggerManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessors"/> class.
        /// </summary>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStates" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessors(
            IStreamProcessorStates streamProcessorStates,
            IExecutionContextManager executionContextManager,
            ILoggerManager loggerManager,
            ILogger<StreamProcessors> logger)
        {
            _streamProcessors = new ConcurrentDictionary<StreamProcessorId, AbstractStreamProcessor>();
            _streamProcessorStates = streamProcessorStates;
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
                return new SuccessfulStreamProcessorRegistration(streamProcessor, tenant);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to register Stream Processor with Id: '{streamProcessorId}' for Tenant: '{tenant}'", streamProcessorId);
                return new FailedStreamProcessorRegistration($"Failed to register Stream Processor with Id: '{streamProcessorId}'. {ex.Message}", tenant);
            }
        }

        async Task<AbstractStreamProcessor> CreateStreamProcessor(
            StreamProcessorId streamProcessorId,
            StreamDefinition streamDefinition,
            TenantId tenant,
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            CancellationToken cancellationToken)
        {
            void unregister() => _streamProcessors.TryRemove(streamProcessorId, out var _);
            if (streamDefinition.Partitioned)
            {
                return await CreatePartitionedStreamProcessor(streamProcessorId, streamDefinition.StreamId, tenant, eventProcessor, eventsFromStreamsFetcher, unregister, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await CreateUnpartitionedStreamProcessor(streamProcessorId, streamDefinition.StreamId, tenant, eventProcessor, eventsFromStreamsFetcher, unregister, cancellationToken).ConfigureAwait(false);
            }
        }

        async Task<Partitioned.StreamProcessor> CreatePartitionedStreamProcessor(
            StreamProcessorId streamProcessorId,
            StreamId sourceStreamId,
            TenantId tenant,
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            Action unregister,
            CancellationToken cancellationToken)
        {
            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(streamProcessorId, cancellationToken).ConfigureAwait(false);
            var streamProcessorState = tryGetStreamProcessorState.streamProcessorState;
            if (!tryGetStreamProcessorState.success)
            {
                streamProcessorState = StreamProcessorState.New;
                await _streamProcessorStates.Persist(streamProcessorId, streamProcessorState, cancellationToken).ConfigureAwait(false);
            }

            if (!streamProcessorState.Partitioned) throw new ExpectedPartitionedStreamProcessorState(streamProcessorId);

            return new Partitioned.StreamProcessor(
                tenant,
                sourceStreamId,
                streamProcessorState as Partitioned.StreamProcessorState,
                eventProcessor,
                _streamProcessorStates,
                eventsFromStreamsFetcher,
                new FailingPartitions(_streamProcessorStates, eventsFromStreamsFetcher, _loggerManager.CreateLogger<FailingPartitions>()),
                unregister,
                _loggerManager.CreateLogger<Partitioned.StreamProcessor>(),
                cancellationToken);
        }

        async Task<StreamProcessor> CreateUnpartitionedStreamProcessor(
            StreamProcessorId streamProcessorId,
            StreamId sourceStreamId,
            TenantId tenant,
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            Action unregister,
            CancellationToken cancellationToken)
        {
            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(streamProcessorId, cancellationToken).ConfigureAwait(false);
            var streamProcessorState = tryGetStreamProcessorState.streamProcessorState;
            if (!tryGetStreamProcessorState.success)
            {
                streamProcessorState = StreamProcessorState.New;
                await _streamProcessorStates.Persist(streamProcessorId, streamProcessorState, cancellationToken).ConfigureAwait(false);
            }

            if (streamProcessorState.Partitioned) throw new ExpectedUnpartitionedStreamProcessorState(streamProcessorId);
            return new StreamProcessor(
                tenant,
                sourceStreamId,
                streamProcessorState as StreamProcessorState,
                eventProcessor,
                _streamProcessorStates,
                eventsFromStreamsFetcher,
                unregister,
                _loggerManager.CreateLogger<StreamProcessor>(),
                cancellationToken);
        }
    }
}