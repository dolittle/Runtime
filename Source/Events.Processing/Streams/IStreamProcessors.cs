// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a hub for <see cref="AbstractScopedStreamProcessor" />.
    /// </summary>
    public interface IStreamProcessors
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamDefinition">The <see cref="IStreamDefinition" /> of the stream that the <see cref="AbstractScopedStreamProcessor" /> is processing.</param>
        /// <param name="getEventProcessor">The <see cref="Func{TResult}" /> <see cref="IEventProcessor" />.</param>
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