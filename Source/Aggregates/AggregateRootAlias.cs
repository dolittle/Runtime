// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Aggregates;

/// <summary>
/// Represents a name alias of an Aggregate Root
/// </summary>
public record AggregateRootAlias(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Gets the <see cref="AggregateRootAlias"/> to use when none is provided by the Client.
    /// </summary>
    public static AggregateRootAlias NotSet => "No alias";
        
    /// <summary>
    /// Implicitly convert from <see cref="string"/> to <see cref="AggregateRootAlias"/>.
    /// </summary>
    /// <param name="alias"><see cref="string"/> representation.</param>
    public static implicit operator AggregateRootAlias(string alias) => new(alias);
}