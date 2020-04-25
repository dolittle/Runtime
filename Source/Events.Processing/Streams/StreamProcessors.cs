// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        readonly IStreamDefinitionRepository _streamDefinitionRepository;
        readonly IStreamProcessorStateRepository _streamProcessorStateRepository;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessors"/> class.
        /// </summary>
        /// <param name="streamDefinitionRepository">The <see cref="IStreamDefinitionRepository" />.</param>
        /// <param name="streamProcessorStateRepository">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessors(
            IStreamDefinitionRepository streamDefinitionRepository,
            IStreamProcessorStateRepository streamProcessorStateRepository,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _streamDefinitionRepository = streamDefinitionRepository;
            _streamProcessors = new ConcurrentDictionary<StreamProcessorId, StreamProcessor>();
            _streamProcessorStateRepository = streamProcessorStateRepository;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<StreamProcessor> Processors =>
            _streamProcessors.Select(_ => _.Value);

        /// <inheritdoc />
        public async Task<StreamProcessorRegistrationResult> Register(
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            StreamId sourceStreamId,
            CancellationToken cancellationToken)
        {
            var tenant = _executionContextManager.Current.Tenant;
            var hasStreamDefinition = await _streamDefinitionRepository.HasFor(eventProcessor.Scope, sourceStreamId, cancellationToken).ConfigureAwait(false);
            if (!hasStreamDefinition)
            {
                _logger.Warning("No persisted stream definition for Stream: '{sourceStreamId}' in Scope: '{scope}' in Tenant: '{tenant}'", sourceStreamId, eventProcessor.Scope);
                return new StreamProcessorRegistrationResult($"No persisted stream definition for Stream: '{sourceStreamId}' in Scope: '{eventProcessor.Scope}' in Tenant: '{tenant}'");
            }

            var streamDefinition = await _streamDefinitionRepository.GetFor(eventProcessor.Scope, sourceStreamId, cancellationToken).ConfigureAwait(false);
            var streamProcessorId = new StreamProcessorId(eventProcessor.Scope, eventProcessor.Identifier, sourceStreamId);

            if (_streamProcessors.ContainsKey(streamProcessorId))
            {
                _logger.Warning("Stream Processor with Id: '{streamProcessorId}' in Tenant: '{tenant}' already registered", streamProcessorId);
                return new StreamProcessorRegistrationResult($"Stream Processor with Id: '{streamProcessorId}' in Tenant: '{tenant}' already registered");
            }
#pragma warning disable CA2000
            var streamProcessor = new StreamProcessor(
                tenant,
                sourceStreamId,
                eventProcessor,
                new StreamProcessorStates(
                    new FailingPartitions(_streamProcessorStateRepository, eventsFromStreamsFetcher, _logger),
                    _streamProcessorStateRepository,
                    _logger),
                eventsFromStreamsFetcher,
                () => Unregister(streamProcessorId),
                _logger,
                cancellationToken);
#pragma warning restore CA2000
            if (_streamProcessors.TryAdd(streamProcessorId, streamProcessor))
            {
                _logger.Trace("Stream Processor with Id: '{streamProcessorId}' registered", streamProcessorId);
                return new StreamProcessorRegistrationResult(streamProcessor);
            }

            _logger.Trace("Stream Processor with Id: '{streamProcessorId}' in Tenant: '{tenant}' could not be registered", streamProcessorId);
            return new StreamProcessorRegistrationResult($"Stream Processor with Id: '{streamProcessorId}' in Tenant: '{tenant}' could not be registered");
        }

        /// <inheritdoc/>
        public void Unregister(StreamProcessorId streamProcessorId)
        {
            if (_streamProcessors.TryRemove(streamProcessorId, out var streamProcessor))
            {
                _logger.Debug($"Removing and disposing of Stream Processor with Id: '{streamProcessorId}'");
                streamProcessor.Dispose();
            }
        }
    }
}