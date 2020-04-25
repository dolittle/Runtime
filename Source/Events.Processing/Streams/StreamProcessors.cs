// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        /// <inheritdoc/>
        public IEnumerable<StreamProcessor> Processors =>
            _streamProcessors.Select(_ => _.Value);

        /// <inheritdoc />
        public StreamProcessorRegistrationResult Register(
            IEventProcessor eventProcessor,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            StreamId sourceStreamId,
            CancellationToken cancellationToken)
        {
            var tenant = _executionContextManager.Current.Tenant;
            var streamProcessorId = new StreamProcessorId(eventProcessor.Scope, eventProcessor.Identifier, sourceStreamId);

            if (_streamProcessors.ContainsKey(streamProcessorId))
            {
                return new StreamProcessorRegistrationResult(false, _streamProcessors[streamProcessorId]);
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
                this,
                _logger,
                cancellationToken);
            if (_streamProcessors.TryAdd(streamProcessorId, streamProcessor))
            {
                return new StreamProcessorRegistrationResult(true, streamProcessor);
            }

            return new StreamProcessorRegistrationResult(false, _streamProcessors[streamProcessorId]);
        }

        /// <inheritdoc/>
        public void Unregister(StreamProcessorId streamProcessorId)
        {
            if (_streamProcessors.TryRemove(streamProcessorId, out var streamProcessor))
            {
                _logger.Debug($"Disposing Stream Processor with key '{streamProcessorId}'");
                streamProcessor.Dispose();
            }
        }
    }
}