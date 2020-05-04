// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessors" />.
    /// </summary>
    [Singleton]
    public class EventProcessors : IEventProcessors
    {
        readonly IStreamProcessors _streamProcessors;
        readonly ILogger<EventProcessors> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessors"/> class.
        /// </summary>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessors(IStreamProcessors streamProcessors, ILogger<EventProcessors> logger)
        {
            _streamProcessors = streamProcessors;
            _logger = logger;
        }

        /// <inheritdoc/>
        public EventProcessorRegistrationResult Register(ScopeId scopeId, EventProcessorId eventProcessorId, StreamId sourceStreamId, Func<IEventProcessor> getEventProcessor, CancellationToken cancellationToken)
        {
            return new EventProcessorRegistrationResult(
                _streamProcessors.TryRegister(
                scopeId,
                eventProcessorId,
                sourceStreamId,
                getEventProcessor,
                cancellationToken,
                out var streamProcessor),
                streamProcessor);
        }

        /// <inheritdoc/>
        public EventProcessorRegistrationResult Register(ScopeId scopeId, EventProcessorId eventProcessorId, StreamDefinition streamDefinition, Func<IEventProcessor> getEventProcessor, CancellationToken cancellationToken)
        {
            return new EventProcessorRegistrationResult(
                _streamProcessors.TryRegister(
                scopeId,
                eventProcessorId,
                streamDefinition,
                getEventProcessor,
                cancellationToken,
                out var streamProcessor),
                streamProcessor);
        }
    }
}