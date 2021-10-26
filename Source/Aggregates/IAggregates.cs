// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.AggregateRoots;

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
        /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of an Aggregate Root with Aggregates.</returns>
        Task<IEnumerable<(AggregateRoot, IEnumerable<Aggregate>)>> GetAll();
        
        /// <summary>
        /// Gets all Aggregates for a specific Aggregate Root.
        /// </summary>
        /// <param name="aggregateRoot">The Aggregate Root to get Aggregates for.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns an <see cref="IEnumerable{T}"/> of Aggregates.</returns>
        Task<IEnumerable<Aggregate>> GetFor(AggregateRoot aggregateRoot);
    }
}
