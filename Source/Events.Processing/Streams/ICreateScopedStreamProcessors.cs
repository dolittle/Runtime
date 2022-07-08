// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Defines a system that can create <see cref="AbstractScopedStreamProcessor" />.
/// </summary>
public interface ICreateScopedStreamProcessors
{
    /// <summary>
    /// Create a <see cref="AbstractScopedStreamProcessor" /> processing the Stream defines by the <see cref="IStreamDefinition" />.
    /// </summary>
    /// <param name="streamDefinition">The definition of the stream to create a scoped stream processor for.</param>
    /// <param name="streamProcessorId">The identifier of the stream processor.</param>
    /// <param name="eventProcessor">The event processor to use for processing events.</param>
    /// <param name="executionContext">The execution context of the stream processor.</param>
    /// <param name="cancellationToken">The cancellation token that is cancelled when the scoped stream processor should stop processing events..</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the created <see cref="AbstractScopedStreamProcessor" />.</returns>
    Task<AbstractScopedStreamProcessor> Create(
        IStreamDefinition streamDefinition,
        IStreamProcessorId streamProcessorId,
        IEventProcessor eventProcessor,
        ExecutionContext executionContext,
        CancellationToken cancellationToken);
}
