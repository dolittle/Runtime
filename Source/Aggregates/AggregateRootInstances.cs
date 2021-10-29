// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregateRootInstances"/>.
    /// </summary>
    [SingletonPerTenant]
    public class AggregateRootInstances : IAggregateRootInstances
    {
        readonly IAggregateRoots _aggregateRoots;
        readonly IFetchAggregateRootInstances _aggregateRootInstancesFetcher;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootInstances"/> class.
        /// </summary>
        /// <param name="aggregateRoots">The Aggregate Roots.</param>
        /// <param name="aggregateRootInstancesFetcher">The system that can fetch Aggregates.</param>
        public AggregateRootInstances(IAggregateRoots aggregateRoots, IFetchAggregateRootInstances aggregateRootInstancesFetcher)
        {
            _aggregateRoots = aggregateRoots;
            _aggregateRootInstancesFetcher = aggregateRootInstancesFetcher;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AggregateRootWithInstances>> GetAll()
        {
            var roots = _aggregateRoots.All;
            var results = new List<AggregateRootWithInstances>();

            foreach (var root in roots)
            {
                results.Add(new AggregateRootWithInstances(root, await FetchInstances(root).ConfigureAwait(false)));
            }
            return results;
        }

        /// <inheritdoc />
        public Task<IEnumerable<AggregateRootInstance>> GetFor(AggregateRoot aggregateRoot) => GetFor(aggregateRoot.Type.Id);

        /// <inheritdoc />
        public Task<IEnumerable<AggregateRootInstance>> GetFor(ArtifactId aggregateRootId)
            => _aggregateRoots.TryGet(aggregateRootId, out var aggregateRoot)
                ? FetchInstances(aggregateRoot)
                : Task.FromResult(Enumerable.Empty<AggregateRootInstance>());

        async Task<IEnumerable<AggregateRootInstance>> FetchInstances(AggregateRoot root)
        {
            var instances = await _aggregateRootInstancesFetcher.FetchFor(root.Type.Id).ConfigureAwait(false);
            return instances.Select(_ => new AggregateRootInstance(_.Item1, _.Item2));
        }
    }
}
