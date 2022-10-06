// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing;

/// <summary>
/// Defines a system that processes an event.
/// </summary>
public interface IEventProcessor
{
    /// <summary>
    /// Gets the <see cref="Scope" />.
    /// </summary>
    ScopeId Scope { get; }

    /// <summary>
    /// Gets the identifier for the <see cref="IEventProcessor"/>.
    /// </summary>
    EventProcessorId Identifier { get; }

    /// <summary>
    /// Processes a batch of <see cref="StreamEvent">events</see>.
    /// </summary>
    /// <param name="batch">The <see cref="IReadOnlyList{T}"/> <see cref="StreamEvent" /> batch to process.</param>
    /// <param name="executionContext">The execution context to process the event in.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns><see cref="IProcessingResult" />.</returns>
    Task<IProcessingResult> Process(IReadOnlyList<StreamEvent> batch, ExecutionContext executionContext, CancellationToken cancellationToken);
    
    /// <summary>
    /// Processes a single <see cref="StreamEvent">event</see>.
    /// </summary>
    /// <param name="streamEvent">The <see cref="StreamEvent" /> event to process.</param>
    /// <param name="executionContext">The execution context to process the event in.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns><see cref="IProcessingResult" />.</returns>
    Task<IProcessingResult> Process(StreamEvent streamEvent, ExecutionContext executionContext, CancellationToken cancellationToken);

    /// <summary>
    /// Reprocesses a batch of <see cref="StreamEvent">events</see>.
    /// </summary>
    /// <param name="batch">The <see cref="IReadOnlyList{T}"/> <see cref="StreamEvent" /> batch to process.</param>
    /// <param name="failureReason">The reason the processor was failing.</param>
    /// <param name="retryCount">The retry count.</param>
    /// <param name="executionContext">The execution context to process the event in.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns><see cref="IProcessingResult" />.</returns>
    Task<IProcessingResult> ReProcess(IReadOnlyList<StreamEvent> batch, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken);
    
    /// <summary>
    /// Reprocesses a single <see cref="StreamEvent">event</see>.
    /// </summary>
    /// <param name="streamEvent">The<see cref="StreamEvent" /> event to process.</param>
    /// <param name="failureReason">The reason the processor was failing.</param>
    /// <param name="retryCount">The retry count.</param>
    /// <param name="executionContext">The execution context to process the event in.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns><see cref="IProcessingResult" />.</returns>
    Task<IProcessingResult> ReProcess(StreamEvent streamEvent, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken);
}
