// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a system for creating and registering a <see cref="StreamProcessor"/> for an <see cref="IEventProcessor"/>.
    /// </summary>
    /// <remarks>
    /// The registration ensures that there is only one <see cref="StreamProcessor"/> for each <see cref="EventProcessorId"/> at any given time.
    /// It also synchronizes the execution of the processing for all tenants.
    /// </remarks>
    public interface IStreamProcessors
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamDefinition">The <see cref="IStreamDefinition" /> of the stream that the <see cref="AbstractScopedStreamProcessor" /> is processing.</param>
        /// <param name="getEventProcessor">The <see cref="FactoryFor{TResult}" /> <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <param name="streamProcessor">The registered <see cref="StreamProcessor" />.</param>
        /// <returns>A value indicating whether a new <see cref="StreamProcessor" /> was registered.</returns>
        Try<StreamProcessor> TryCreateAndRegister(
            ScopeId scopeId,
            EventProcessorId eventProcessorId,
            IStreamDefinition sourceStreamDefinition,
            Func<IEventProcessor> getEventProcessor,
            CancellationToken cancellationToken);
    }
}