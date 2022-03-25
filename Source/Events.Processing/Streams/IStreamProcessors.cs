// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams;

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
    /// Tries to create and register a new instance of <see cref="StreamProcessor"/>.
    /// </summary>
    /// <param name="scopeId">The scope that the processor should process events from.</param>
    /// <param name="eventProcessorId">The identifier of the event processor.</param>
    /// <param name="sourceStreamDefinition">The definition of the stream (in the specified scope) that the stream processor should process events from.</param>
    /// <param name="createEventProcessor">The factory to use to create the event processor to call for a tenant.</param>
    /// <param name="executionContext">The execution context to use for the created stream processor.</param>
    /// <param name="cancellationToken">The cancellation token that will be cancelled to indicate that the created stream processor should stop processing.</param>
    /// <returns>A <see cref="Try{TResult}"/> that contains the newly created and registered <see cref="StreamProcessor"/> if successful.</returns>
    Try<StreamProcessor> TryCreateAndRegister(
        ScopeId scopeId,
        EventProcessorId eventProcessorId,
        IStreamDefinition sourceStreamDefinition,
        Func<TenantId, IEventProcessor> createEventProcessor,
        ExecutionContext executionContext,
        CancellationToken cancellationToken);


    /// <summary>
    /// Reprocesses all events for a <see cref="StreamProcessor"/> from a <see cref="StreamPosition" /> for a tenant.
    /// </summary>
    /// <param name="streamProcessorId">The <see cref="StreamProcessorId"/> of the <see cref="StreamProcessor"/>.</param>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="position">The <see cref="StreamPosition" />.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
    Task<Try<StreamPosition>> ReprocessEventsFrom(StreamProcessorId streamProcessorId, TenantId tenant, StreamPosition position);
        
        
    /// <summary>
    /// Reprocesses all the events for the <see cref="StreamProcessor"/> for all tenants.
    /// </summary>
    /// <param name="streamProcessorId">The <see cref="StreamProcessorId"/> of the <see cref="StreamProcessor"/>.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
    Task<Try<IDictionary<TenantId, Try<StreamPosition>>>> ReprocessAllEvents(StreamProcessorId streamProcessorId);
}
