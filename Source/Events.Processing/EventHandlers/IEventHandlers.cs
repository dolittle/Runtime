// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Defines a system that knows how to register and start event handlers.
    /// </summary>
    public interface IEventHandlers
    {
        /// <summary>
        /// Registers and starts processing an event handler.
        /// </summary>
        /// <typeparam name="TResponse">The response <see cref="IMessage" />.</typeparam>
        /// <typeparam name="TRequest">The request <see cref="IMessage" />.</typeparam>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="types">The list of <see cref="ArtifactId" /> event types it handles.</param>
        /// <param name="partitioned">Whether the event handler is partitioned.</param>
        /// <param name="dispatcher">The call dispatcher.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A task.</returns>
        Task RegisterAndStartProcessing<TResponse, TRequest>(
            ScopeId scope,
            EventProcessorId eventProcessorId,
            StreamId sourceStream,
            IEnumerable<ArtifactId> types,
            bool partitioned,
            IReverseCallDispatcher<TResponse, TRequest> dispatcher,
            IEventProcessor eventProcessor,
            CancellationToken cancellationToken)
            where TResponse : IMessage
            where TRequest : IMessage;
    }
}