// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Old.Embeddings;
using Dolittle.Runtime.Projections.Store;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters
{
    public class EmbeddingAggregatesConverter : IConvertFromOldToNew<AggregateRoot, AggregateRoot>
    {
        readonly IConvertOldEventSourceId _oldEventSourceIdConverter;
        readonly IDictionary<EmbeddingId, IEnumerable<ProjectionKey>> _embeddings;

        public EmbeddingAggregatesConverter(IConvertOldEventSourceId oldEventSourceIdConverter, IDictionary<EmbeddingId, IEnumerable<ProjectionKey>> embeddings)
        {
            _oldEventSourceIdConverter = oldEventSourceIdConverter;
            _embeddings = embeddings;
            Filter = Builders<AggregateRoot>.Filter.In(_ => _.AggregateType, _embeddings.Keys.Select(_ => _.Value));
        }

        public FilterDefinition<AggregateRoot> Filter { get; }

        public AggregateRoot Convert(AggregateRoot old)
            => new(
                _oldEventSourceIdConverter.Convert(Guid.Parse(old.EventSource), _embeddings[old.AggregateType]),
                old.AggregateType,
                old.Version);
    }
}