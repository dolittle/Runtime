// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Defines a system that can fetch Aggregate Root Instances from the Event Store.
    /// </summary>
    public interface IFetchAggregateRootInstances
    {
        /// <summary>
        /// Gets all Aggregates for an Aggregate Root.
        /// </summary>
        /// <param name="aggregateRoot">The Aggregate Root to get Aggregates from.</param>
        public Task<IEnumerable<AggregateRootInstance>> FetchFor(AggregateRoot aggregateRoot);
    }
}
