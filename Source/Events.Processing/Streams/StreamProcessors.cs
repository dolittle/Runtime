// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
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
        public async Task<StreamProcessorRegistration> Register(
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            StreamId sourceStreamId,
            CancellationToken cancellationToken)
        {
            var tenant = _executionContextManager.Current.Tenant;
            var streamProcessorId = new StreamProcessorId(eventProcessor.Scope, eventProcessor.Identifier, sourceStreamId);
            try
            {
                var hasStreamDefinition = await HasStreamDefinitionFor(eventProcessor.Scope, sourceStreamId, cancellationToken).ConfigureAwait(false);
                if (!hasStreamDefinition)
                {
                    _logger.Warning("No persisted stream definition for Stream: '{sourceStreamId}' in Scope: '{scope}' for Tenant: '{tenant}'", sourceStreamId, eventProcessor.Scope, tenant);
                    return new FailedStreamProcessorRegistration($"No persisted stream definition for Stream: '{sourceStreamId}' in Scope: '{eventProcessor.Scope}'", tenant);
                }

                var streamDefinition = await GetStreamDefinitionFor(eventProcessor.Scope, sourceStreamId, cancellationToken).ConfigureAwait(false);

                var streamProcessor = new StreamProcessor(
                    tenant,
                    sourceStreamId,
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

        Task<bool> HasStreamDefinitionFor(ScopeId scopeId, StreamId sourceStreamId, CancellationToken cancellationToken)
        {
            if (IsEventLogStream(scopeId, sourceStreamId)) return Task.FromResult(true);
            return _streamDefinitionRepository.HasFor(scopeId, sourceStreamId, cancellationToken);
        }

        Task<StreamDefinition> GetStreamDefinitionFor(ScopeId scopeId, StreamId sourceStreamId, CancellationToken cancellationToken)
        {
            if (IsEventLogStream(scopeId, sourceStreamId)) return Task.FromResult(StreamDefinition.EventLog);
            return _streamDefinitionRepository.GetFor(scopeId, sourceStreamId, cancellationToken);
        }

        bool IsEventLogStream(ScopeId scopeId, StreamId sourceStreamId) => scopeId == ScopeId.Default && sourceStreamId == StreamId.AllStreamId;
    }
}