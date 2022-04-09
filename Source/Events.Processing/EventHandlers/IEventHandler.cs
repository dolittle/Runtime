// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

public interface IEventHandler : IDisposable
{
    /// <summary>
    /// Gets the <see cref="StreamId">target stream</see> for the <see cref="EventHandler"/>.
    /// </summary>
    StreamId TargetStream { get; }

    /// <summary>
    /// Gets the <see cref="EventHandlerInfo"/> for the <see cref="EventHandler"/>.
    /// </summary>
    EventHandlerInfo Info { get; }

    /// <summary>
    /// Gets the <see cref="Scope"/> for the <see cref="EventHandler"/>.
    /// </summary>
    ScopeId Scope { get; }

    /// <summary>
    /// Gets the <see cref="EventProcessorId"/> for the <see cref="EventHandler"/>.
    /// </summary>
    EventProcessorId EventProcessor { get; }

    /// <summary>
    /// Gets the <see cref="ArtifactId"/> for the <see cref="EventHandler"/>.
    /// </summary>
    IEnumerable<ArtifactId> EventTypes { get; }

    /// <summary>
    /// Gets whether or not the <see cref="EventHandler"/> is partitioned.
    /// </summary>
    bool Partitioned { get; }

    /// <summary>
    /// Gets the <see cref="StreamDefinition"/> for the filtered stream.
    /// </summary>
    StreamDefinition FilteredStreamDefinition { get; }

    /// <summary>
    /// Gets the <see cref="TypeFilterWithEventSourcePartitionDefinition"/> for the filter.
    /// </summary>
    TypeFilterWithEventSourcePartitionDefinition FilterDefinition { get; }

    /// <summary>
    /// Gets the <see cref="StreamProcessor"/> for the filter.
    /// </summary>
    StreamProcessor FilterStreamProcessor { get; }

    /// <summary>
    /// Gets the <see cref="StreamProcessor"/> for the event processor.
    /// </summary>
    StreamProcessor EventProcessorStreamProcessor { get; }

    /// <summary>
    /// Gets the current state of the Event Handler. 
    /// </summary>
    /// <returns>The  <see cref="IStreamProcessorState"/> per <see cref="TenantId"/>.</returns>
    Try<IDictionary<TenantId, IStreamProcessorState>> GetEventHandlerCurrentState();

    /// <summary>
    /// Reprocesses all events from a <see cref="StreamPosition" /> for a tenant.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/>.</param>
    /// <param name="position">The <see cref="StreamPosition" />.</param>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to.</returns>
    Task<Try<StreamPosition>> ReprocessEventsFrom(TenantId tenant, StreamPosition position);

    /// <summary>
    /// Reprocesses all the events for all tenants.
    /// </summary>
    /// <returns>The <see cref="Task"/> that, when resolved, returns a <see cref="Dictionary{TKey,TValue}"/> with a <see cref="Try{TResult}"/> with the <see cref="StreamPosition"/> it was set to for each <see cref="TenantId"/>.</returns>
    Task<Try<IDictionary<TenantId, Try<StreamPosition>>>> ReprocessAllEvents();

    /// <summary>
    /// Register and start the event handler for filtering and processing.
    /// </summary>
    /// <returns>Async <see cref="Task"/>.</returns>
    Task RegisterAndStart();
}
