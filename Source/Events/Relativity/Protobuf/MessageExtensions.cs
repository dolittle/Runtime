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

namespace Dolittle.Runtime.Events.Relativity.Protobuf
{
    /// <summary>
    /// Extension methods for working with conversion related to <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/>
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static System.Protobuf.guid ToProtobuf(this System.Guid guid)
        {
            var protobufGuid = new System.Protobuf.guid();
            protobufGuid.Value = ByteString.CopyFrom(guid.ToByteArray());
            return protobufGuid;
        }

        /// <summary>
        /// Convert from <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/> to <see cref="Protobuf.CommittedEventStream"/>
        /// </summary>
        /// <param name="committedEventStream"><see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/> to convert from</param>
        /// <param name="tenant"><see cref="TenantId"/> the message is for</param>
        /// <param name="serializer"><see cref="ISerializer"/> to use for serializing content</param>
        /// <returns>The converted <see cref="Protobuf.CommittedEventStream"/></returns>
        public static Protobuf.CommittedEventStream ToMessage(this Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream, TenantId tenant, ISerializer serializer)
        {
            var message = new Protobuf.CommittedEventStream
            {
                Tenant = tenant.ToProtobuf(),
                Source = committedEventStream.Source.ToMessage(),
                Sequence = committedEventStream.Sequence,
                Id = committedEventStream.Id.ToProtobuf(),
                TimeStamp = committedEventStream.Timestamp.ToFileTime(),
                CorrelationId = committedEventStream.CorrelationId.ToProtobuf()
            };

            committedEventStream.Events.Select(@event => new Protobuf.EventEnvelope
            {
                Id = @event.Id.ToProtobuf(),
                Metadata = @event.Metadata.ToMessage()
            }).ForEach(message.Events.Add);

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionedEventSource"></param>
        /// <returns></returns>
        public static Protobuf.VersionedEventSource ToMessage(this Dolittle.Runtime.Events.Store.VersionedEventSource versionedEventSource)
        {
            var source = new Protobuf.VersionedEventSource {
                Version = new Protobuf.EventSourceVersion {
                    Commit = versionedEventSource.Version.Commit,
                    Sequence = versionedEventSource.Version.Sequence
                },
                EventSource = versionedEventSource.EventSource.ToProtobuf(),
                Artifact = versionedEventSource.Artifact.ToProtobuf()
            };
            return source;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="artifact"></param>
        /// <returns></returns>
        public static Protobuf.Artifact ToMessage(this Dolittle.Artifacts.Artifact artifact)
        {
            var message = new Protobuf.Artifact();
            message.Id = artifact.Id.ToProtobuf();
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
        /// <param name="concept"></param>
        /// <returns></returns>
        public static System.Protobuf.guid ToProtobuf(this ConceptAs<Guid> concept)
        {
            return concept.Value.ToProtobuf();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Protobuf.EventMetadata ToMessage(this Dolittle.Runtime.Events.Store.EventMetadata metadata)
        {
            var message = new Protobuf.EventMetadata();
            message.Source = metadata.VersionedEventSource.ToMessage();
            message.CorrelationId = metadata.CorrelationId.ToProtobuf();
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
        /// <param name="guid"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToConcept<T>(this System.Protobuf.guid guid) where T:ConceptAs<Guid>
        {
            return ConceptFactory.CreateConceptInstance(typeof(T), new Guid(guid.Value.ToByteArray())) as T;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Dolittle.Runtime.Events.Store.EventSourceVersion ToEventSourceVersion(this EventSourceVersion message)
        {
            return new Dolittle.Runtime.Events.Store.EventSourceVersion(message.Commit, message.Sequence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Dolittle.Runtime.Events.Store.VersionedEventSource ToVersionedEventSource(this VersionedEventSource message)
        {
            return new Dolittle.Runtime.Events.Store.VersionedEventSource(
                message.Version.ToEventSourceVersion(),
                message.EventSource.ToConcept<EventSourceId>(),
                message.Artifact.ToConcept<ArtifactId>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Dolittle.Artifacts.Artifact ToArtifact(this Artifact message)
        {
            return new Dolittle.Artifacts.Artifact(
                message.Id.ToConcept<ArtifactId>(),
                message.Generation
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Dolittle.Runtime.Events.Store.EventMetadata ToEventMetadata(this Protobuf.EventMetadata message)
        {
            var metadata = new Dolittle.Runtime.Events.Store.EventMetadata(
                message.Source.ToVersionedEventSource(),
                message.CorrelationId.ToConcept<CorrelationId>(),
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
        public static Dolittle.Runtime.Events.Store.CommittedEventStream ToCommittedEventStream(this Protobuf.CommittedEventStream message, ISerializer serializer)
        {
            var tenantId = message.Tenant.ToConcept<TenantId>();
            var eventSourceId = message.Source.EventSource.ToConcept<EventSourceId>();
            var artifactId = message.Source.Artifact.ToConcept<ArtifactId>();
            var versionedEventSource = new Dolittle.Runtime.Events.Store.VersionedEventSource(eventSourceId, artifactId);
            var commitId = message.Id.ToConcept<CommitId>();
            var correlationId = message.CorrelationId.ToConcept<CorrelationId>();
            var timeStamp = DateTimeOffset.FromFileTime(message.TimeStamp);
            
            var events = message.Events.Select(_ =>                 
                new Dolittle.Runtime.Events.Store.EventEnvelope(
                    _.Id.ToConcept<EventId>(),
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