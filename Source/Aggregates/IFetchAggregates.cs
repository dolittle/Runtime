// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.AggregateRoots;

namespace Dolittle.Runtime.Aggregates
{
    public interface IFetchAggregates
    {
        /// <summary>
        /// Gets all Aggregates for an Aggregate Root.
        /// </summary>
        /// <param name="aggregateRoot">The Aggregate Root to get Aggregates from.</param>
        public Task<IEnumerable<Aggregate>> FetchFor(AggregateRoot aggregateRoot);
    }

    public record AggregateRootAndAggregates(AggregateRoot AggregateRoot, IEnumerable<Aggregate> Aggregates);
}
