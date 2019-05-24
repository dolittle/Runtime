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
using Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion;
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
        /// Convert a <see cref="Runtime.Grpc.Interaction.Protobuf.CommittedEventStream"/> to <see cref="CommittedEventStream"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Runtime.Grpc.Interaction.Protobuf.CommittedEventStream"/> to convert from</param>
        /// <returns>Converted <see cref="CommittedEventStream"/></returns>
        public static CommittedEventStream ToCommittedEventStream(this Runtime.Grpc.Interaction.Protobuf.CommittedEventStream protobuf)
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
        /// Convert from <see cref="CommittedEventStream"/> to <see cref="Runtime.Grpc.Interaction.Protobuf.CommittedEventStream"/>
        /// </summary>
        /// <param name="committedEventStream"><see cref="CommittedEventStream"/> to convert from</param>
        /// <returns>The converted <see cref="Runtime.Grpc.Interaction.Protobuf.CommittedEventStream"/></returns>
        public static Runtime.Grpc.Interaction.Protobuf.CommittedEventStream ToProtobuf(this CommittedEventStream committedEventStream)
        {
            var protobuf = new Runtime.Grpc.Interaction.Protobuf.CommittedEventStream
            {
                Sequence = committedEventStream.Sequence,
                Source = committedEventStream.Source.ToProtobuf(),
                Id = committedEventStream.Id.ToProtobuf(),
                CorrelationId = committedEventStream.CorrelationId.ToProtobuf(),
                TimeStamp = committedEventStream.Timestamp.ToUnixTimeMilliseconds()
            };

            committedEventStream.Events.Select(@event =>
            {
                var envelope = new Runtime.Grpc.Interaction.Protobuf.EventEnvelope
                {
                    Metadata = @event.Metadata.ToProtobuf()
                };
                envelope.Event = @event.Event.ToProtobuf();
                
                return envelope;
            }).ForEach(protobuf.Events.Add);

            return protobuf;
        }


        /// <summary>
        /// Convert from <see cref="CommittedEventStreamWithContext"/> to <see cref="Runtime.Grpc.Interaction.Protobuf.CommittedEventStream"/>
        /// </summary>
        /// <param name="contextualEventStream"><see cref="CommittedEventStreamWithContext"/> to convert from</param>
        /// <returns>Converted <see cref="Runtime.Grpc.Interaction.Protobuf.CommittedEventStream"/></returns>
        public static Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext ToProtobuf(this CommittedEventStreamWithContext contextualEventStream)
        {
            var protobuf = new Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext
            {
                Commit = contextualEventStream.EventStream.ToProtobuf(),
                Context = contextualEventStream.Context.ToProtobuf()
            };

            return protobuf;
        }      
        /// <summary>
        /// Convert from <see cref="Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext"/> to <see cref="CommittedEventStreamWithContext"/>
        /// </summary>
        /// <param name="protobuf"><see cref="Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext"/> to convert from</param>
        /// <returns>Converted <see cref="CommittedEventStreamWithContext"/></returns>
        public static CommittedEventStreamWithContext ToCommittedEventStreamWithContext(this Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext protobuf)
        {
            var context = protobuf.Context.ToExecutionContext();
            var committedEventStream = protobuf.Commit.ToCommittedEventStream();

            return new CommittedEventStreamWithContext(committedEventStream, context);
        }
    }
}