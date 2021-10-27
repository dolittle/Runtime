// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregates"/>.
    /// </summary>
    [SingletonPerTenant]
    public class Aggregates : IAggregates
    {
        readonly IAggregateRoots _aggregateRoots;
        readonly IFetchAggregates _aggregatesFetcher;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregates"/> class.
        /// </summary>
        /// <param name="aggregateRoots">The Aggregate Roots.</param>
        /// <param name="aggregatesFetcher">The system that can fetch Aggregates.</param>
        public Aggregates(IAggregateRoots aggregateRoots, IFetchAggregates aggregatesFetcher)
        {
            _aggregateRoots = aggregateRoots;
            _aggregatesFetcher = aggregatesFetcher;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Aggregate>> GetAll()
        {
            var roots = _aggregateRoots.All;
            var results = new List<Aggregate>();

            foreach (var root in roots)
            {
                var aggregates = await _aggregatesFetcher.FetchFor(root).ConfigureAwait(false);
                results.AddRange(aggregates);
            }
            return results;
        }

        /// <inheritdoc />
        public Task<IEnumerable<Aggregate>> GetFor(AggregateRoot aggregateRoot) => GetFor(aggregateRoot.Type.Id);

        /// <inheritdoc />
        public Task<IEnumerable<Aggregate>> GetFor(ArtifactId aggregateRootId)
            => _aggregateRoots.TryGet(aggregateRootId, out var aggregateRoot)
                ? _aggregatesFetcher.FetchFor(aggregateRoot)
                : Task.FromResult(Enumerable.Empty<Aggregate>());
    }
}
