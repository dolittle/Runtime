// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

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
    /// Registers a <see cref="StreamProcessor" />.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId" />.</param>
    /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
    /// <param name="sourceStreamDefinition">The <see cref="IStreamDefinition" /> of the stream that the <see cref="AbstractScopedStreamProcessor" /> is processing.</param>
    /// <param name="getEventProcessor">The <see cref="Func{TArg, TResult}" /> <see cref="IEventProcessor" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A value indicating whether a new <see cref="StreamProcessor" /> was registered.</returns>
    Try<StreamProcessor> TryCreateAndRegister(
        ScopeId scopeId,
        EventProcessorId eventProcessorId,
        IStreamDefinition sourceStreamDefinition,
        Func<IServiceProvider, IEventProcessor> getEventProcessor,
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
