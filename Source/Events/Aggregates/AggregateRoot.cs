// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Aggregates;

/// <summary>
/// Represents an Aggregate Root.
/// </summary>
/// <param name="Identifier">The Aggregate Root identifier.</param>
/// <param name="Alias">The alias of the Aggregate Root.</param>
public record AggregateRoot(AggregateRootId Identifier, AggregateRootAlias Alias)
{
    /// <summary>
    /// Initializes an new instance of the <see cref="AggregateRoot"/> record.
    /// </summary>
    /// <param name="identifier">The Aggregate Root identifier.</param>
    public AggregateRoot(AggregateRootId identifier)
        : this(identifier, AggregateRootAlias.NotSet)
    { }
}
