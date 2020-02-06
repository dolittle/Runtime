// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        readonly TaskFactory _factory;

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
            _factory = new TaskFactory(TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.DenyChildAttach);
        }

        /// <inheritdoc/>
        public IEnumerable<StreamProcessor> Processors => _streamProcessors.Select(_ => _.Value);

        /// <inheritdoc />
        public void Register(IEventProcessor eventProcessor, StreamId sourceStreamId)
        {
            var tenant = _executionContextManager.Current.Tenant;
            var streamProcessor = new StreamProcessor(
                sourceStreamId,
                eventProcessor,
                _streamProcessorStateRepository,
                _eventsFromStreamsFetcher,
                _logger);

            if (_streamProcessors.TryAdd(streamProcessor.Identifier, streamProcessor))
            {
                #pragma warning disable CA2008
                _factory.StartNew(streamProcessor.BeginProcessing);
                _logger.Debug($"Started Stream Processor with key '{new StreamProcessorId(eventProcessor.Identifier, sourceStreamId)}' for tenant '{tenant}'");
            }
            else
            {
                throw new StreamProcessorKeyAlreadyRegistered(streamProcessor.Identifier);
            }
        }
    }
}