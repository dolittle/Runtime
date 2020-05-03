// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Async;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamProcessors" />.
    /// </summary>
    [Singleton]
    public class StreamProcessors : IStreamProcessors
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
        readonly ConcurrentDictionary<StreamProcessorId, StreamProcessor> _streamProcessors;
        readonly FactoryFor<IEventFetchers> _getEventFetchers;
        readonly ILoggerManager _loggerManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessors"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="getStreamProcessorStates">The <see cref="FactoryFor{T}" /> <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="getEventFetchers">The <see cref="FactoryFor{T}" /> <see cref="IEventFetchers" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        public StreamProcessors(
            IPerformActionOnAllTenants onAllTenants,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
            FactoryFor<IEventFetchers> getEventFetchers,
            ILoggerManager loggerManager)
        {
            _onAllTenants = onAllTenants;
            _getStreamProcessorStates = getStreamProcessorStates;
            _streamProcessors = new ConcurrentDictionary<StreamProcessorId, StreamProcessor>();
            _getEventFetchers = getEventFetchers;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<StreamProcessors>();
        }

        /// <inheritdoc />
        public async Task<Try<StreamProcessor>> TryRegister(
            IStreamDefinition streamDefinition,
            IEventProcessor eventProcessor,
            CancellationToken cancellationToken)
        {
            var streamProcessorId = new StreamProcessorId(eventProcessor.Scope, eventProcessor.Identifier, streamDefinition.StreamId);
            try
            {
                if (!_streamProcessors.ContainsKey(streamProcessorId))
                {
                    _logger.Warning("Stream Processor with Id: '{streamProcessorId}' already registered", streamProcessorId);
                    return false;
                }

                var streamProcessor = new StreamProcessor(
                    streamProcessorId,
                    _onAllTenants,
                    streamDefinition,
                    eventProcessor,
                    () => _streamProcessors.TryRemove(streamProcessorId, out var _),
                    _getStreamProcessorStates,
                    _getEventFetchers,
                    _loggerManager,
                    cancellationToken);
                if (!_streamProcessors.TryAdd(streamProcessorId, streamProcessor))
                {
                    _logger.Warning("Stream Processor with Id: '{streamProcessorId}' already registered", streamProcessorId);
                    return false;
                }

                _logger.Trace("Stream Processor with Id: '{streamProcessorId}' registered for Tenant: '{tenant}'", streamProcessorId);
                return streamProcessor;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to register Stream Processor with Id: '{streamProcessorId}' for Tenant: '{tenant}'", streamProcessorId);
                return false;
            }
        }
    }
}