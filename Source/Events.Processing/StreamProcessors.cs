// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessors" />.
    /// </summary>
    [SingletonPerTenant]
    public class StreamProcessors : IStreamProcessors
    {
        readonly ConcurrentDictionary<StreamProcessorId, StreamProcessor> _streamProcessors;
        readonly IStreamProcessorStateRepository _streamProcessorStateRepository;
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Processing.StreamProcessors"/> class.
        /// </summary>
        /// <param name="streamProcessorStateRepository">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="eventsFromStreamsFetcher">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessors(
            IStreamProcessorStateRepository streamProcessorStateRepository,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _streamProcessors = new ConcurrentDictionary<StreamProcessorId, StreamProcessor>();
            _streamProcessorStateRepository = streamProcessorStateRepository;
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<StreamProcessor> Processors => _streamProcessors.Select(_ => _.Value);

        /// <inheritdoc />
        public StreamProcessor Register(IEventProcessor eventProcessor, StreamId sourceStreamId)
        {
            var tenant = _executionContextManager.Current.Tenant;
            var streamProcessor = new StreamProcessor(
                tenant,
                sourceStreamId,
                eventProcessor,
                _streamProcessorStateRepository,
                _eventsFromStreamsFetcher,
                _logger);

            if (_streamProcessors.TryAdd(streamProcessor.Identifier, streamProcessor))
            {
                streamProcessor.Start();
                _logger.Debug($"Started Stream Processor with key '{new StreamProcessorId(eventProcessor.Identifier, sourceStreamId)}' for tenant '{tenant}'");
                return streamProcessor;
            }

            throw new StreamProcessorKeyAlreadyRegistered(streamProcessor.Identifier);
        }

        /// <inheritdoc/>
        public void Unregister(EventProcessorId eventProcessorId, StreamId sourceStreamId)
        {
            var identifier = new StreamProcessorId(eventProcessorId, sourceStreamId);
            if (_streamProcessors.TryRemove(identifier, out var streamProcessor))
            {
                _logger.Debug($"Stopping Stream Processor with key '{identifier}'");
                streamProcessor.Stop();
                streamProcessor.Dispose();
            }
        }
    }
}