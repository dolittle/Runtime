// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchAggregateRootInstances"/>
    /// </summary>
    public class AggregateRootInstancesFetcher : IFetchAggregateRootInstances
    {
        readonly FilterDefinitionBuilder<AggregateRoot> _filter = Builders<AggregateRoot>.Filter;
        readonly IAggregatesCollection _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootInstancesFetcher"/> class.
        /// </summary>
        /// <param name="aggregates">The <see cref="IAggregatesCollection" />.</param>
        public AggregateRootInstancesFetcher(IAggregatesCollection aggregates)
        {
            _aggregates = aggregates;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<(EventSourceId, AggregateRootVersion)>> FetchFor(ArtifactId aggregateRoot)
        {
            var aggregates = await _aggregates
                .Aggregates
                .Find(_filter.Eq(_ => _.AggregateType, aggregateRoot.Value))
                .ToListAsync().ConfigureAwait(false);

            return aggregates.Select(_ => (new EventSourceId(_.EventSource), new AggregateRootVersion(_.Version)));
        }
    }
}
