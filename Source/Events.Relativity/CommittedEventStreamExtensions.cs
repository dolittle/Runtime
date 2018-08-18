/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Concepts;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Tenancy;
using Dolittle.Serialization.Protobuf;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Extension methods for working with conversion related to <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/>
    /// </summary>
    public static class CommittedEventStreamExtensions
    {

        /// <summary>
        /// Convert from <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/> to <see cref="CommittedEventStreamParticleMessage"/>
        /// </summary>
        /// <param name="committedEventStream"><see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/> to convert from</param>
        /// <param name="tenant"><see cref="TenantId"/> the message is for</param>
        /// <param name="serializer"><see cref="ISerializer"/> to use for serializing content</param>
        /// <returns>The converted <see cref="CommittedEventStreamParticleMessage"/></returns>
        public static CommittedEventStreamParticleMessage ToMessage(this Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream, TenantId tenant, ISerializer serializer)
        {
            var message = new CommittedEventStreamParticleMessage
            {
                Tenant = ByteString.CopyFrom(tenant.Value.ToByteArray()),
                Source = committedEventStream.Source.ToMessage(),
                Sequence = committedEventStream.Sequence,
                Id = ByteString.CopyFrom(committedEventStream.Id.Value.ToByteArray()),
                TimeStamp = committedEventStream.Timestamp.ToFileTime(),
                CorrelationId = ByteString.CopyFrom(committedEventStream.CorrelationId.Value.ToByteArray()),
            };

            committedEventStream.Events.Select(@event => new EventParticleMessage
            {
                Id = ByteString.CopyFrom(@event.Id.Value.ToByteArray()),
                Metadata = @event.Metadata.ToMessage()
                    
            }).ForEach(message.Events.Add);

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionedEventSource"></param>
        /// <returns></returns>
        public static VersionedEventSourceMessage ToMessage(this VersionedEventSource versionedEventSource)
        {
            var source = new VersionedEventSourceMessage {
                Version = new EventSourceVersionMessage {
                    Commit = versionedEventSource.Version.Commit,
                    Sequence = versionedEventSource.Version.Sequence
                },
                EventSource = ByteString.CopyFrom(versionedEventSource.EventSource.Value.ToByteArray()),
                Artifact = ByteString.CopyFrom(versionedEventSource.Artifact.Value.ToByteArray())
            };
            return source;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="artifact"></param>
        /// <returns></returns>
        public static ArtifactMessage ToMessage(this Artifact artifact)
        {
            var message = new ArtifactMessage();
            message.Id = ByteString.CopyFrom(artifact.Id.Value.ToByteArray());
            message.Generation = artifact.Generation.Value;

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static ByteString ToByteString(this ConceptAs<Guid> guid)
        {
            return ByteString.CopyFrom(guid.Value.ToByteArray());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static EventMetadataMessage ToMessage(this EventMetadata metadata)
        {
            var message = new EventMetadataMessage();
            message.Source = metadata.VersionedEventSource.ToMessage();
            message.CorrelationId = metadata.CorrelationId.ToByteString();
            message.Artifact = metadata.Artifact.ToMessage();
            message.CausedBy = metadata.CausedBy;
            message.Occurred = metadata.Occurred.ToFileTime();
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteString"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToGuidConcept<T>(this ByteString byteString) where T:class
        {
            return ConceptFactory.CreateConceptInstance(typeof(T), new Guid(byteString.ToByteArray())) as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Dolittle.Runtime.Events.Store.EventSourceVersion ToEventSourceVersion(this EventSourceVersionMessage message)
        {
            return new Dolittle.Runtime.Events.Store.EventSourceVersion(message.Commit, message.Sequence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static VersionedEventSource ToVersionedEventSource(this VersionedEventSourceMessage message)
        {
            return new VersionedEventSource(
                message.Version.ToEventSourceVersion(),
                message.EventSource.ToGuidConcept<EventSourceId>(),
                message.Artifact.ToGuidConcept<ArtifactId>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Artifact ToArtifact(this ArtifactMessage message)
        {
            return new Artifact(
                message.Id.ToGuidConcept<ArtifactId>(),
                message.Generation
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static EventMetadata ToEventMetadata(this EventMetadataMessage message)
        {
            var metadata = new EventMetadata(
                message.Source.ToVersionedEventSource(),
                message.CorrelationId.ToGuidConcept<CorrelationId>(),
                message.Artifact.ToArtifact(),
                message.CausedBy,
                DateTimeOffset.FromFileTime(message.Occurred)
            );
            return metadata;
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static Dolittle.Runtime.Events.Store.CommittedEventStream ToCommittedEventStream(this CommittedEventStreamParticleMessage message, ISerializer serializer)
        {
            var tenantId = (TenantId)new Guid(message.Tenant.ToByteArray());
            var eventSourceId = (EventSourceId)new Guid(message.Source.EventSource.ToByteArray());
            var artifactId = (ArtifactId)new Guid(message.Source.Artifact.ToByteArray());
            var versionedEventSource = new VersionedEventSource(eventSourceId, artifactId);
            var commitId = (CommitId)new Guid(message.Id.ToByteArray());
            var correlationId = (CorrelationId)new Guid(message.CorrelationId.ToByteArray());
            var timeStamp = DateTimeOffset.FromFileTime(message.TimeStamp);
            
            var events = message.Events.Select(_ =>                 
                new Dolittle.Runtime.Events.Store.EventEnvelope(
                    (EventId)new Guid(_.Id.ToByteArray()),
                    _.Metadata.ToEventMetadata(),
                    new PropertyBag(new Dictionary<string, object>())
                )
            ).ToArray();
            
            return new Dolittle.Runtime.Events.Store.CommittedEventStream(
                message.Sequence,
                versionedEventSource,
                commitId,
                correlationId,
                timeStamp,
                new Dolittle.Runtime.Events.Store.EventStream(events)
            );
        }
    }
}