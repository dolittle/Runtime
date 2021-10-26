// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Aggregates.AggregateRoots
{
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

    }
}
