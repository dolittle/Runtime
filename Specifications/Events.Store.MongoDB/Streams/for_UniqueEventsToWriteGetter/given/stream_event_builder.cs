// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson;
using StreamEvent = Dolittle.Runtime.Events.Store.MongoDB.Events.StreamEvent;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.given;

public static class a_stream_event
{
    public static builder at_event_log_position(EventLogSequenceNumber sequence_number) => new builder(sequence_number); 

    public class builder
    {
        readonly EventLogSequenceNumber event_log_sequence_number;
        PartitionId partition;
        EventSourceId event_source;
        ArtifactId event_type_id = Guid.Empty;
        ArtifactGeneration event_type_generation;

        public builder(EventLogSequenceNumber event_log_sequence_number) => this.event_log_sequence_number = event_log_sequence_number;

        public builder with_partition(PartitionId partition)
        {
            this.partition = partition;
            return this;
        }
        
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
        
        public Events.StreamEvent build()
            => new StreamEvent(
                0,
                partition,
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
                new StreamEventMetadata(event_log_sequence_number, DateTime.Now, event_source, event_type_id, event_type_generation, false),
                new AggregateMetadata(),
                new EventHorizonMetadata(),
                new BsonDocument());

        public static implicit operator Events.StreamEvent(builder builder) => builder.build();
    }
    
}