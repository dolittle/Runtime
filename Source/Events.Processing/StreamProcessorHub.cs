// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessorHub" />.
    /// </summary>
    [Singleton]
    public class StreamProcessorHub : IStreamProcessorHub
    {
        readonly ConcurrentDictionary<StreamProcessorKey, ConcurrentDictionary<TenantId, StreamProcessor>> _streamProcessors =
            new ConcurrentDictionary<StreamProcessorKey, ConcurrentDictionary<TenantId, StreamProcessor>>();

        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorHub"/> class.
        /// </summary>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public StreamProcessorHub(IExecutionContextManager executionContextManager, ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Register(IEventProcessor eventProcessor, StreamId sourceStreamId, IStreamProcessorStateRepository streamProcessorStateRepository, IFetchNextEvent nextEventFetcher, ExecutionContext executionContext)
        {
            var tenant = _executionContextManager.Current.Tenant;
            var streamProcessor = new StreamProcessor(sourceStreamId, eventProcessor, streamProcessorStateRepository, nextEventFetcher, _logger);
            _logger.Debug($"Created Stream Processor with key '{streamProcessor.Key}' for tenant '{tenant}'");
            _streamProcessors.GetOrAdd(streamProcessor.Key, (_) => new ConcurrentDictionary<TenantId, StreamProcessor>())
                .AddOrUpdate(executionContext.Tenant, streamProcessor, (_, __) => streamProcessor);

            var task = streamProcessor.Start();
        }
    }
}