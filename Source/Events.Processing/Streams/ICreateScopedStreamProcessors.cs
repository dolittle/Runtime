// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a system that can create <see cref="AbstractScopedStreamProcessor" />.
    /// </summary>
    public interface ICreateScopedStreamProcessors
    {
        /// <summary>
        /// Create a <see cref="AbstractScopedStreamProcessor" /> processing the Stream defines by the <see cref="IStreamDefinition" />.
        /// </summary>
        /// <param name="streamDefinition">The <see cref="IStreamDefinition" />.</param>
        /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the created <see cref="AbstractScopedStreamProcessor" />.</returns>
        Task<AbstractScopedStreamProcessor> Create(
            IStreamDefinition streamDefinition,
            IStreamProcessorId streamProcessorId,
            IEventProcessor eventProcessor,
            CancellationToken cancellationToken);
    }
}
