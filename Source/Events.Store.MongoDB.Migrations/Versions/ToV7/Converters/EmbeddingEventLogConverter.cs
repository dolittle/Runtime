// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Old.Embeddings;
using Dolittle.Runtime.Projections.Store;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters
{
    public class EmbeddingEventLogConverter : IConvertFromOldToNew<Events.Event, Events.Event>
    {
        readonly IConvertOldEventSourceId _oldEventSourceIdConverter;
        readonly IEnumerable<ProjectionKey> _keys;

        public EmbeddingEventLogConverter(IConvertOldEventSourceId oldEventSourceIdConverter, EmbeddingId embedding, IEnumerable<ProjectionKey> keys)
        {
            _oldEventSourceIdConverter = oldEventSourceIdConverter;
            _keys = keys;
            Filter = Builders<Events.Event>.Filter.Eq(_ => _.Aggregate.TypeId, embedding.Value);
        }

        public FilterDefinition<Events.Event> Filter { get; }

        public Events.Event Convert(Events.Event old)
            => new (
                old.EventLogSequenceNumber,
                old.ExecutionContext,
                new Events.EventMetadata(
                    old.Metadata.Occurred,
                    _oldEventSourceIdConverter.Convert(Guid.Parse(old.Metadata.EventSource), _keys),
                    old.Metadata.TypeId,
                    old.Metadata.TypeGeneration,
                    old.Metadata.Public),
                old.Aggregate,
                old.EventHorizon,
                old.Content);
    }
}