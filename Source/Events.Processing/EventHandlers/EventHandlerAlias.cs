// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;
namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents a name alias of an Event Handler
/// </summary>
public record EventHandlerAlias(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Gets the <see cref="EventHandlerAlias"/> to use when none is provided by the Client.
    /// </summary>
    public static EventHandlerAlias NotSet => "No alias";
        
    /// <summary>
    /// Implicitly convert from <see cref="string"/> to <see cref="EventHandlerAlias"/>.
    /// </summary>
    /// <param name="alias"><see cref="string"/> representation.</param>
    public static implicit operator EventHandlerAlias(string alias) => new(alias);
}