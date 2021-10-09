// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Old.Embeddings;
using Dolittle.Runtime.Projections.Store;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters
{
    public class EmbeddingStreamConverter : IConvertFromOldToNew<Events.StreamEvent, Events.StreamEvent>
    {
        readonly IConvertOldEventSourceId _oldEventSourceIdConverter;
        readonly IDictionary<EmbeddingId, IEnumerable<ProjectionKey>> _embeddings;

        public EmbeddingStreamConverter(IConvertOldEventSourceId oldEventSourceIdConverter, IDictionary<EmbeddingId, IEnumerable<ProjectionKey>> embeddings)
        {
            _oldEventSourceIdConverter = oldEventSourceIdConverter;
            _embeddings = embeddings;
            Filter = Builders<Events.StreamEvent>.Filter.In(_ => _.Aggregate.TypeId, _embeddings.Keys.Select(_ => _.Value));
        }

        public FilterDefinition<Events.StreamEvent> Filter { get; }

        public Events.StreamEvent Convert(Events.StreamEvent old)
        {
            var key = _oldEventSourceIdConverter.Convert(Guid.Parse(old.Metadata.EventSource), _embeddings[old.Aggregate.TypeId]);
            return new Events.StreamEvent(
                old.StreamPosition,
                old.Partition.Equals(old.Metadata.EventSource)
                    ? key.Value
                    : old.Partition,
                old.ExecutionContext,
                new Events.StreamEventMetadata(
                    old.Metadata.EventLogSequenceNumber,
                    old.Metadata.Occurred,
                    key.Value,
                    old.Metadata.TypeId,
                    old.Metadata.TypeGeneration,
                    old.Metadata.Public),
                old.Aggregate,
                old.EventHorizon,
                old.Content);
        }
    }
}