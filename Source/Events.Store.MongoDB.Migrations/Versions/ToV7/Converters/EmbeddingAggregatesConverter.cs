// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
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
        readonly IEnumerable<ProjectionKey> _keys;

        public EmbeddingAggregatesConverter(IConvertOldEventSourceId oldEventSourceIdConverter, EmbeddingId embedding, IEnumerable<ProjectionKey> keys)
        {
            _oldEventSourceIdConverter = oldEventSourceIdConverter;
            _keys = keys;
            Filter = Builders<AggregateRoot>.Filter.Eq(_ => _.AggregateType, embedding.Value);
        }

        public FilterDefinition<AggregateRoot> Filter { get; }

        public AggregateRoot Convert(AggregateRoot old)
            => new(
                _oldEventSourceIdConverter.Convert(Guid.Parse(old.EventSource), _keys),
                old.AggregateType,
                old.Version);
    }
}