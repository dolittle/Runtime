// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Aggregates;

/// <summary>
/// Defines a system that knows about Aggregate Root Instances.
/// </summary>
public interface IAggregateRootInstances
{
    /// <summary>
    /// Gets all Aggregates for all Aggregate Roots.
    /// </summary>
    /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of type <see cref="AggregateRootWithInstances"/>.</returns>
    Task<IEnumerable<AggregateRootWithInstances>> GetAll();
        
    /// <summary>
    /// Gets all Aggregates for a specific Aggregate Root.
    /// </summary>
    /// <param name="identifier">The Aggregate Root to get Aggregate Root Instances for.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="AggregateRootWithInstances"/>.</returns>
    Task<AggregateRootWithInstances> GetFor(AggregateRootId identifier);
}