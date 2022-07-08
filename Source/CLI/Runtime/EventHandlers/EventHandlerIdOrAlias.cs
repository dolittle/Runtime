// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Represents an Event Handler Id or the Alias and Scope of an Event Handler.
/// </summary>
public record EventHandlerIdOrAlias
{
    readonly ScopeId _scope = ScopeId.Default;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerIdOrAlias"/>.
    /// </summary>
    /// <param name="alias">The Event Handler Alias.</param>
    /// <param name="scope">The Event Handler Scope.</param>
    public EventHandlerIdOrAlias(EventHandlerAlias alias, ScopeId scope)
    {
        Alias = alias;
        _scope = scope;
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerIdOrAlias"/>.
    /// </summary>
    /// <param name="id">The Event Handler Id.</param>
    public EventHandlerIdOrAlias(EventHandlerId id)
    {
        Id = id;
    }
        
    /// <summary>
    /// Gets the Event Handler Alias.
    /// </summary>
    public EventHandlerAlias Alias { get; }
        
    /// <summary>
    /// Gets the Event Handler Id
    /// </summary>
    public EventHandlerId Id { get; }
        
    /// <summary>
    /// Gets the Event Handler Scope.
    /// </summary>
    public ScopeId Scope => Id?.Scope ?? _scope;
        
    /// <summary>
    /// Gets a value indicating whether this represents the Event Handler Alias or not.
    /// </summary>
    public bool IsAlias => Alias != default;
}