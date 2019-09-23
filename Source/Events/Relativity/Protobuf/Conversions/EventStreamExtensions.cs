/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Tenancy;
using Dolittle.Time;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for event streams to convert to and from protobuf representations
    /// </summary>
    public static class EventStreamExtensions
    {
        /// <summary>
        /// Convert a <see cref="Interaction.Grpc.CommittedEventStream"/> to <see cref="CommittedEventStream"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Interaction.Grpc.CommittedEventStream"/> to convert from</param>
        /// <returns>Converted <see cref="CommittedEventStream"/></returns>
        public static CommittedEventStream ToCommittedEventStream(this Interaction.Grpc.CommittedEventStream protobuf)
        {
            var eventSourceId = protobuf.Source.EventSource.ToConcept<EventSourceId>();
            var artifactId = protobuf.Source.Artifact.ToConcept<ArtifactId>();
            var versionedEventSource = new VersionedEventSource(eventSourceId, artifactId);
            var commitId = protobuf.Id.ToConcept<CommitId>();
            var correlationId = protobuf.CorrelationId.ToConcept<CorrelationId>();
            var timeStamp = protobuf.TimeStamp.ToDateTimeOffset();
            
            var events = protobuf.Events.Select(_ =>                 
                new EventEnvelope(
                    _.Metadata.ToEventMetadata(),
                    _.Event.ToPropertyBag()
                )
            ).ToArray();

            return new CommittedEventStream(
                protobuf.Sequence,
                versionedEventSource,
                commitId,
                correlationId,
                timeStamp,
                new EventStream(events)
            );
        }

        /// <summary>
        /// Convert from <see cref="CommittedEventStream"/> to <see cref="Interaction.Grpc.CommittedEventStream"/>
        /// </summary>
        /// <param name="committedEventStream"><see cref="CommittedEventStream"/> to convert from</param>
        /// <returns>The converted <see cref="Interaction.Grpc.CommittedEventStream"/></returns>
        public static Interaction.Grpc.CommittedEventStream ToProtobuf(this CommittedEventStream committedEventStream)
        {
            var protobuf = new Interaction.Grpc.CommittedEventStream
            {
                Sequence = committedEventStream.Sequence,
                Source = committedEventStream.Source.ToProtobuf(),
                Id = committedEventStream.Id.ToProtobuf(),
                CorrelationId = committedEventStream.CorrelationId.ToProtobuf(),
                TimeStamp = committedEventStream.Timestamp.ToUnixTimeMilliseconds()
            };

            committedEventStream.Events.Select(@event =>
            {
                var envelope = new Interaction.Grpc.EventEnvelope
                {
                    Metadata = @event.Metadata.ToProtobuf()
                };
                envelope.Event = @event.Event.ToProtobuf();
                
                return envelope;
            }).ForEach(protobuf.Events.Add);

            return protobuf;
        }


        /// <summary>
        /// Convert from <see cref="CommittedEventStreamWithContext"/> to <see cref="Interaction.Grpc.CommittedEventStream"/>
        /// </summary>
        /// <param name="contextualEventStream"><see cref="CommittedEventStreamWithContext"/> to convert from</param>
        /// <returns>Converted <see cref="Interaction.Grpc.CommittedEventStream"/></returns>
        public static Interaction.Grpc.CommittedEventStreamWithContext ToProtobuf(this CommittedEventStreamWithContext contextualEventStream)
        {
            var protobuf = new Interaction.Grpc.CommittedEventStreamWithContext
            {
                Commit = contextualEventStream.EventStream.ToProtobuf(),
                Context = contextualEventStream.Context.ToProtobuf()
            };

            return protobuf;
        }      
        /// <summary>
        /// Convert from <see cref="Interaction.Grpc.CommittedEventStreamWithContext"/> to <see cref="CommittedEventStreamWithContext"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Interaction.Grpc.CommittedEventStreamWithContext"/> to convert from</param>
        /// <returns>Converted <see cref="CommittedEventStreamWithContext"/></returns>
        public static CommittedEventStreamWithContext ToCommittedEventStreamWithContext(this Interaction.Grpc.CommittedEventStreamWithContext protobuf)
        {
            var context = protobuf.Context.ToExecutionContext();
            var committedEventStream = protobuf.Commit.ToCommittedEventStream();

            return new CommittedEventStreamWithContext(committedEventStream, context);
        }
    }
}