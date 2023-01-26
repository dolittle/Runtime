// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents a name alias of a Projection.
/// </summary>
public record ProjectionAlias(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Gets the <see cref="ProjectionAlias"/> to use when none is provided by the Client.
    /// </summary>
    public static ProjectionAlias NotSet => "No alias";
        
    /// <summary>
    /// Implicitly convert from <see cref="string"/> to <see cref="ProjectionAlias"/>.
    /// </summary>
    /// <param name="alias"><see cref="string"/> representation.</param>
    public static implicit operator ProjectionAlias(string alias) => new(alias);
}
