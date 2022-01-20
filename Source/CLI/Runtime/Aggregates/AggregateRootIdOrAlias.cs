// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates;

/// <summary>
/// Represents an Aggregate Root Id or the Alias of an Aggregate Root.
/// </summary>
public record AggregateRootIdOrAlias
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootIdOrAlias"/>.
    /// </summary>
    /// <param name="alias">The Aggregate Root Alias.</param>
    /// <param name="scope">The Aggregate Root Scope.</param>
    public AggregateRootIdOrAlias(AggregateRootAlias alias)
    {
        Alias = alias;
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootIdOrAlias"/>.
    /// </summary>
    /// <param name="id">The Aggregate Root Id.</param>
    public AggregateRootIdOrAlias(ArtifactId id)
    {
        Id = id;
    }
        
    /// <summary>
    /// Gets the Aggregate Root Alias.
    /// </summary>
    public AggregateRootAlias Alias { get; }
        
    /// <summary>
    /// Gets the Aggregate Root Id
    /// </summary>
    public ArtifactId Id { get; }

    /// <summary>
    /// Gets a value indicating whether this represents Aggregate Root Alias or not.
    /// </summary>
    public bool IsAlias => Alias != default;
}