// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Defines a system that knows about Aggregates.
    /// </summary>
    public interface IAggregateRootInstances
    {
        /// <summary>
        /// Gets all Aggregates for all Aggregate Roots.
        /// </summary>
        /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of Aggregate Root with Instances.</returns>
        Task<IEnumerable<AggregateRootWithInstances>> GetAll();
        
        /// <summary>
        /// Gets all Aggregates for a specific Aggregate Root.
        /// </summary>
        /// <param name="aggregateRoot">The Aggregate Root to get Aggregate Root Instances for.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of Aggregate Root Instances.</returns>
        Task<IEnumerable<AggregateRootInstance>> GetFor(AggregateRoot aggregateRoot);
        
        /// <summary>
        /// Gets all Aggregates for a specific Aggregate Root.
        /// </summary>
        /// <param name="aggregateRootId">The Aggregate Root to get Aggregate Root Instances for.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of Aggregate Root Instances.</returns>
        Task<IEnumerable<AggregateRootInstance>> GetFor(ArtifactId aggregateRootId);
    }

    public record AggregateRootWithInstances(AggregateRoot Root, IEnumerable<AggregateRootInstance> Instances);
}
