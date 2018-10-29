/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Runtime.Events.Store;
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
        /// Convert a <see cref="CommittedEventStream"/> to <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/>
        /// </summary>
        /// <param name="protobuf"><see cref="CommittedEventStream"/> to convert from</param>
        /// <returns>Converted <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/></returns>
        public static Dolittle.Runtime.Events.Store.CommittedEventStream ToCommittedEventStream(this CommittedEventStream protobuf)
        {
            var eventSourceId = protobuf.Source.EventSource.ToConcept<EventSourceId>();
            var artifactId = protobuf.Source.Artifact.ToConcept<ArtifactId>();
            var versionedEventSource = new Dolittle.Runtime.Events.VersionedEventSource(eventSourceId, artifactId);
            var commitId = protobuf.Id.ToConcept<CommitId>();
            var correlationId = protobuf.CorrelationId.ToConcept<CorrelationId>();
            var timeStamp = protobuf.TimeStamp.ToDateTimeOffset();
            
            var events = protobuf.Events.Select(_ =>                 
                new Dolittle.Runtime.Events.EventEnvelope(
                    _.Metadata.ToEventMetadata(),
                    _.Event.ToPropertyBag()
                )
            ).ToArray();

            return new Dolittle.Runtime.Events.Store.CommittedEventStream(
                protobuf.Sequence,
                versionedEventSource,
                commitId,
                correlationId,
                timeStamp,
                new Dolittle.Runtime.Events.Store.EventStream(events)
            );
        }

        /// <summary>
        /// Convert from <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/> to <see cref="Protobuf.CommittedEventStream"/>
        /// </summary>
        /// <param name="committedEventStream"><see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/> to convert from</param>
        /// <returns>The converted <see cref="Protobuf.CommittedEventStream"/></returns>
        public static CommittedEventStream ToProtobuf(this Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream)
        {
            var protobuf = new Protobuf.CommittedEventStream
            {
                Sequence = committedEventStream.Sequence,
                Source = committedEventStream.Source.ToProtobuf(),
                Id = committedEventStream.Id.ToProtobuf(),
                CorrelationId = committedEventStream.CorrelationId.ToProtobuf(),
                TimeStamp = committedEventStream.Timestamp.ToUnixTimeMilliseconds()
            };

            committedEventStream.Events.Select(@event =>
            {
                var envelope = new EventEnvelope
                {
                    Metadata = @event.Metadata.ToProtobuf()
                };
                envelope.Event.Add(@event.Event.ToProtobuf());
                
                return envelope;
            }).ForEach(protobuf.Events.Add);

            return protobuf;
        }


        /// <summary>
        /// Convert from <see cref="Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext"/> to <see cref="Protobuf.CommittedEventStream"/>
        /// </summary>
        /// <param name="contextualEventStream"><see cref="Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext"/> to convert from</param>
        /// <returns>Converted <see cref="Protobuf.CommittedEventStream"/></returns>
        public static CommittedEventStreamWithContext ToProtobuf(this Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext contextualEventStream)
        {
            var protobuf = new CommittedEventStreamWithContext
            {
                Commit = contextualEventStream.EventStream.ToProtobuf(),
                Context = contextualEventStream.Context.ToProtobuf()
            };

            contextualEventStream.EventStream.Events.Select(@event =>
            {
                var envelope = new EventEnvelope
                {
                    Metadata = @event.Metadata.ToProtobuf(),
                };

                envelope.Event.Add(@event.Event.ToProtobuf());
                
                return envelope;
            }).ForEach(protobuf.Commit.Events.Add);

            return protobuf;
        }        
    }
}