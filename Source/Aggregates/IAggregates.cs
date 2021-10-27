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
    public interface IAggregates
    {
        /// <summary>
        /// Gets all Aggregates for all Aggregate Roots.
        /// </summary>
        /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of Aggregates.</returns>
        Task<IEnumerable<Aggregate>> GetAll();
        
        /// <summary>
        /// Gets all Aggregates for a specific Aggregate Root.
        /// </summary>
        /// <param name="aggregateRoot">The Aggregate Root to get Aggregates for.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of Aggregates.</returns>
        Task<IEnumerable<Aggregate>> GetFor(AggregateRoot aggregateRoot);
        
        /// <summary>
        /// Gets all Aggregates for a specific Aggregate Root.
        /// </summary>
        /// <param name="aggregateRootId">The Aggregate Root to get Aggregates for.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of Aggregates.</returns>
        Task<IEnumerable<Aggregate>> GetFor(ArtifactId aggregateRootId);
    }
}
