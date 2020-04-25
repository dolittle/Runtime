// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessors" />.
    /// </summary>
    [SingletonPerTenant]
    public class StreamProcessors : IStreamProcessors
    {
        readonly ConcurrentDictionary<StreamProcessorId, StreamProcessor> _streamProcessors;
        readonly IStreamProcessorStateRepository _streamProcessorStateRepository;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessors"/> class.
        /// </summary>
        /// <param name="streamProcessorStateRepository">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessors(
            IStreamProcessorStateRepository streamProcessorStateRepository,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _streamProcessors = new ConcurrentDictionary<StreamProcessorId, StreamProcessor>();
            _streamProcessorStateRepository = streamProcessorStateRepository;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc />
        public StreamProcessorRegistration Register(
            StreamDefinition streamDefinition,
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            CancellationToken cancellationToken)
        {
            var tenant = _executionContextManager.Current.Tenant;
            var streamProcessorId = new StreamProcessorId(eventProcessor.Scope, eventProcessor.Identifier, streamDefinition.StreamId);
            try
            {
                var streamProcessor = new StreamProcessor(
                    tenant,
                    streamDefinition.StreamId,
                    eventProcessor,
                    new StreamProcessorStates(
                        new FailingPartitions(_streamProcessorStateRepository, eventsFromStreamsFetcher, _logger),
                        _streamProcessorStateRepository,
                        _logger),
                    eventsFromStreamsFetcher,
                    _logger,
                    cancellationToken);
                if (!_streamProcessors.TryAdd(streamProcessorId, streamProcessor))
                {
                    _logger.Warning("Stream Processor with Id: '{streamProcessorId}' for Tenant: '{tenant}' already registered", streamProcessorId, tenant);
                    return new FailedStreamProcessorRegistration($"Stream Processor with Id: '{streamProcessorId}' already registered", tenant);
                }

                _logger.Trace("Stream Processor with Id: '{streamProcessorId}' registered for Tenant: '{tenant}'", tenant);
                return new SuccessfulStreamProcessorRegistration(streamProcessor, tenant, cancellationToken.Register(() => Unregister(streamProcessorId)));
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
    }
}