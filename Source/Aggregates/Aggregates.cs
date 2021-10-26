// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.AggregateRoots;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.Aggregates
{
    [SingletonPerTenant]
    public class Aggregates : IAggregates
    {
        readonly IAggregateRoots _aggregateRoots;
        readonly IFetchAggregates _aggregatesFetcher;

        public Aggregates(IAggregateRoots aggregateRoots, IFetchAggregates aggregatesFetcher)
        {
            _aggregateRoots = aggregateRoots;
            _aggregatesFetcher = aggregatesFetcher;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<(AggregateRoot, IEnumerable<Aggregate>)>> GetAll()
        {
            var roots = _aggregateRoots.All;

            var results = new List<(AggregateRoot, IEnumerable<Aggregate>)>();
            foreach (var root in roots)
            {
                var aggregates = await _aggregatesFetcher.FetchFor(root).ConfigureAwait(false);
                results.Add((root, aggregates));
            }
            return results;
        }

        /// <inheritdoc />
        public Task<IEnumerable<Aggregate>> GetFor(AggregateRoot aggregateRoot)
            => !_aggregateRoots.All.Contains(aggregateRoot) 
                ? Task.FromResult(Enumerable.Empty<Aggregate>())
                : _aggregatesFetcher.FetchFor(aggregateRoot);
    }
}
