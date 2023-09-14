// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents the registration arguments sent from a Client to the Runtime to register an Event Handler.
/// </summary>
public record EventHandlerRegistrationArguments
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerRegistrationArguments"/> class.
    /// </summary>
    /// <param name="executionContext">The ExecutionContext of the Client while registering</param>
    /// <param name="eventHandler">The identifier of the Event Handler</param>
    /// <param name="eventTypes">The Event types that the Event Handler handles.</param>
    /// <param name="partitioned">Whether the Event Handler is partitioned or unpartitioned.</param>
    /// <param name="scope">The Scope the Event Handler will be handling events in.</param>
    /// <param name="startFrom">The position to start from if the event handler does not have state</param>
    /// <param name="concurrency">How many concurrent calls the Event Handler can process simultaneously</param>
    public EventHandlerRegistrationArguments(ExecutionContext executionContext, EventProcessorId eventHandler, IEnumerable<ArtifactId> eventTypes,
        bool partitioned, ScopeId scope, StartFrom startFrom, int concurrency)
    {
        ExecutionContext = executionContext;
        EventHandler = eventHandler;
        EventTypes = eventTypes;
        Partitioned = partitioned;
        Scope = scope;
        Alias = EventHandlerAlias.NotSet;
        HasAlias = false;
        StartFrom = startFrom;
        Concurrency = concurrency > 0 ? concurrency : 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerRegistrationArguments"/> class.
    /// </summary>
    /// <param name="executionContext">The ExecutionContext of the Client while registering</param>
    /// <param name="eventHandler">The identifier of the Event Handler</param>
    /// <param name="eventTypes">The Event types that the Event Handler handles.</param>
    /// <param name="partitioned">Whether the Event Handler is partitioned or unpartitioned.</param>
    /// <param name="scope">The Scope the Event Handler will be handling events in.</param>
    /// <param name="alias">The alias of the Event Handler.</param>
    /// <param name="startFrom">The position to start from if the event handler does not have state</param>
    /// <param name="concurrency">How many concurrent calls the Event Handler can process simultaneously</param>
    public EventHandlerRegistrationArguments(ExecutionContext executionContext, EventProcessorId eventHandler, IEnumerable<ArtifactId> eventTypes,
        bool partitioned, ScopeId scope, EventHandlerAlias alias, StartFrom startFrom, int concurrency = 1)
    {
        ExecutionContext = executionContext;
        EventHandler = eventHandler;
        EventTypes = eventTypes;
        Partitioned = partitioned;
        Scope = scope;
        Alias = alias;
        Concurrency = concurrency > 0 ? concurrency : 1;
        this.StartFrom = startFrom;
        HasAlias = true;
    }

    /// <summary>
    /// How many concurrent calls the Event Handler can process simultaneously.
    /// </summary>
    public int Concurrency { get; set; }

    /// <summary>
    /// Gets the ExecutionContext of the Client while registering.
    /// </summary>
    public ExecutionContext ExecutionContext { get; }

    /// <summary>
    /// Gets the identifier of the Event Handler.
    /// </summary>
    public EventProcessorId EventHandler { get; }

    /// <summary>
    /// Gets the Event types that the Event Handler handles.
    /// </summary>
    public IEnumerable<ArtifactId> EventTypes { get; }

    /// <summary>
    /// Gets a value indicating whether the Event Handler is partitioned or unpartitioned.
    /// </summary>
    public bool Partitioned { get; }

    /// <summary>
    /// Gets the Scope the Event Handler will be handling events in.
    /// </summary>
    public ScopeId Scope { get; }

    /// <summary>
    /// Gets the alias of the Event Handler if set, or <see cref="EventHandlerAlias.NotSet"/> if not passed from the Client.
    /// </summary>
    public EventHandlerAlias Alias { get; }

    /// <summary>
    /// Where to start processing events from, if the event handler does not have state
    /// </summary>
    public StartFrom StartFrom { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the Client passed along an alias for the Event Handler.
    /// </summary>
    public bool HasAlias { get; }
}
