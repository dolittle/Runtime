// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that knows about Event Processors.
    /// </summary>
    public interface IEventProcessors
    {
        /// <summary>
        /// Try to register the Event Processor.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        /// <param name="getEventProcessor">The <see cref="Func{TResult}" /> that returns the <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <param name="streamProcessor">The registered <see cref="StreamProcessor" />.</param>
        /// <returns>A value indicating whether the <see cref="StreamProcessor" /> was registered.</returns>
        bool TryRegisterEventProcessor(ScopeId scopeId, EventProcessorId eventProcessorId, StreamId sourceStreamId, Func<IEventProcessor> getEventProcessor, CancellationToken cancellationToken, out StreamProcessor streamProcessor);

        /// <summary>
        /// Try to register the Event Processor.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="streamDefinition">The <see cref="StreamDefinition" />.</param>
        /// <param name="getEventProcessor">The <see cref="Func{TResult}" /> that returns the <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <param name="streamProcessor">The registered <see cref="StreamProcessor" />.</param>
        /// <returns>A value indicating whether the <see cref="StreamProcessor" /> was registered.</returns>
        bool TryRegisterEventProcessor(ScopeId scopeId, EventProcessorId eventProcessorId, StreamDefinition streamDefinition, Func<IEventProcessor> getEventProcessor, CancellationToken cancellationToken, out StreamProcessor streamProcessor);
    }
}
