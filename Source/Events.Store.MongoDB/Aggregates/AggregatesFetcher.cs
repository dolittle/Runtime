// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchAggregates"/>
    /// </summary>
    public class AggregatesFetcher : IFetchAggregates
    {
        readonly FilterDefinitionBuilder<AggregateRoot> _filter = Builders<AggregateRoot>.Filter;
        readonly IAggregatesCollection _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatesFetcher"/> class.
        /// </summary>
        /// <param name="aggregates">The <see cref="IAggregatesCollection" />.</param>
        public AggregatesFetcher(IAggregatesCollection aggregates)
        {
            _aggregates = aggregates;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Aggregate>> FetchFor(Runtime.Aggregates.AggregateRoot aggregateRoot)
        {
            var aggregates = await _aggregates
                .Aggregates
                .Find(_filter.Eq(_ => _.AggregateType, aggregateRoot.Type.Id.Value))
                .ToListAsync().ConfigureAwait(false);

            return aggregates.Select(_ => new Aggregate(aggregateRoot, _.EventSource, _.Version));
        }
    }
}
