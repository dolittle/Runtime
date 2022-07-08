// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.given;

public static class an_event
{
    public static builder at_event_log_position(EventLogSequenceNumber sequence_number) => new builder(sequence_number); 

    public class builder
    {
        readonly EventLogSequenceNumber event_log_sequence_number;
        EventSourceId event_source;
        ArtifactId event_type_id = Guid.Empty;
        ArtifactGeneration event_type_generation;

        public builder(EventLogSequenceNumber event_log_sequence_number) => this.event_log_sequence_number = event_log_sequence_number;

        public builder with_event_source(EventSourceId event_source)
        {
            this.event_source = event_source;
            return this;
        }

        public builder with_event_type(Artifact artifact)
        {
            event_type_id = artifact.Id;
            event_type_generation = artifact.Generation;
            return this;
        }
        
        public Events.Event build()
            => new Events.Event(
                event_log_sequence_number,
                new ExecutionContext(
                    Guid.Empty,
                    new byte[]
                    {
                    },
                    Guid.Empty,
                    Guid.Empty,
                    new Events.Version(0, 0, 0, 0, ""),
                    "",
                    new Claim[]
                    {
                    }),
                new EventMetadata(DateTime.Now, event_source, event_type_id, event_type_generation, false),
                new AggregateMetadata(),
                new EventHorizonMetadata(),
                new BsonDocument());

        public static implicit operator Events.Event(builder builder) => builder.build();
    }
}