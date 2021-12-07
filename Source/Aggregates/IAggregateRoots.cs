// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Aggregates;

/// <summary>
/// Defines a system that knows about Aggregate Roots.
/// </summary>
public interface IAggregateRoots
{
    /// <summary>
    /// Gets all registered Aggregate Roots.
    /// </summary>
    IEnumerable<AggregateRoot> All { get; }

    /// <summary>
    /// Registers an Aggregate Root.
    /// </summary>
    /// <param name="aggregateRoot">The Aggregate Root to register.</param>
    void Register(AggregateRoot aggregateRoot);

    /// <summary>
    /// Gets a registered Aggregate Root.
    /// </summary>
    /// <param name="aggregateRootId">The Aggregate Root Id.</param>
    /// <param name="aggregateRoot">The registered Aggregate Root.</param>
    /// <returns>A value indicating whether an Aggregate Root with the given Id is registered.</returns>
    bool TryGet(ArtifactId aggregateRootId, out AggregateRoot aggregateRoot);
}