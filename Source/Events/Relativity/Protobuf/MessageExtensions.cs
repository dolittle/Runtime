/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Concepts;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Tenancy;
using Dolittle.Serialization.Protobuf;
using Dolittle.Execution;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
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

        static Types GetTypeFrom(object obj)
        {
            var type = obj.GetType();
            if( type == typeof(string)) return Types.String;
            if( type == typeof(int)) return Types.Int32;
            if( type == typeof(long)) return Types.Int64;
            if( type == typeof(uint)) return Types.UInt32;
            if( type == typeof(long)) return Types.UInt64;
            if( type == typeof(Int32)) return Types.Int32;
            if( type == typeof(Int64)) return Types.Int64;
            if( type == typeof(UInt32)) return Types.UInt32;
            if( type == typeof(UInt64)) return Types.UInt64;
            if( type == typeof(float)) return Types.Float;
            if( type == typeof(double)) return Types.Double;
            if( type == typeof(bool)) return Types.Boolean;
            if( type == typeof(DateTime)) return Types.DateTime;
            if( type == typeof(DateTimeOffset)) return Types.DateTimeOffset;
            if( type == typeof(Guid)) return Types.Guid;

            return Types.Unknown;
        }

        static void WriteObjectWithTypeTo(Types type, object obj, CodedOutputStream stream)
        {
            switch( type )
            {
                case Types.String: stream.WriteString(obj as string); break;
                case Types.Int32: stream.WriteInt32((int)obj); break;
                case Types.Int64: stream.WriteInt64((Int64)obj); break;
                case Types.UInt32: stream.WriteUInt32((uint)obj); break;
                case Types.UInt64: stream.WriteUInt64((UInt64)obj); break;
                case Types.Float: stream.WriteFloat((float)obj); break;
                case Types.Double: stream.WriteDouble((double)obj); break;
                case Types.Boolean: stream.WriteBool((bool)obj); break;
                case Types.DateTime: stream.WriteInt64((Int64)((DateTime)obj).ToFileTime()); break;
                case Types.DateTimeOffset: stream.WriteInt64((Int64)((DateTimeOffset)obj).ToFileTime()); break;
                case Types.Guid: stream.WriteBytes(ByteString.CopyFrom(((Guid)obj).ToByteArray())); break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyBag"></param>
        /// <returns></returns>
        public static MapField<string, System.Protobuf.Object> ToProtobuf(this PropertyBag propertyBag)
        {
            var mapField = new MapField<string, System.Protobuf.Object>();
            propertyBag.ForEach(keyValue => 
            {
                var obj = new System.Protobuf.Object();
                var type = GetTypeFrom(keyValue.Value);;
                obj.Type = (int)type;

                var stream = new MemoryStream();
                using( var outputStream = new CodedOutputStream(stream) )
                {
                    WriteObjectWithTypeTo(type, keyValue.Value, outputStream);
                    outputStream.Flush();
                    stream.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    obj.Content = ByteString.CopyFrom(stream.ToArray());
                }
                
                mapField.Add(keyValue.Key, obj);
            });
            

            return mapField;
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

            committedEventStream.Events.Select(@event =>
            {
                var envelope = new Protobuf.EventEnvelope
                {
                    Id = @event.Id.ToProtobuf(),
                    Metadata = @event.Metadata.ToMessage(),
                };

                envelope.Event.Add(@event.Event.ToProtobuf());
                
                return envelope;
            }).ForEach(message.Events.Add);

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionedEventSource"></param>
        /// <returns></returns>
        public static Protobuf.VersionedEventSource ToMessage(this Dolittle.Runtime.Events.VersionedEventSource versionedEventSource)
        {
            var source = new Protobuf.VersionedEventSource
            {
                Version = new Protobuf.EventSourceVersion
                {
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
        public static Protobuf.EventMetadata ToMessage(this Dolittle.Runtime.Events.EventMetadata metadata)
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
        public static T ToGuidConcept<T>(this ByteString byteString) where T : class
        {
            return ConceptFactory.CreateConceptInstance(typeof(T), new Guid(byteString.ToByteArray())) as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToConcept<T>(this System.Protobuf.guid guid) where T : ConceptAs<Guid>
        {
            return ConceptFactory.CreateConceptInstance(typeof(T), new Guid(guid.Value.ToByteArray())) as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Dolittle.Runtime.Events.EventSourceVersion ToEventSourceVersion(this EventSourceVersion message)
        {
            return new Dolittle.Runtime.Events.EventSourceVersion(message.Commit, message.Sequence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Dolittle.Runtime.Events.VersionedEventSource ToVersionedEventSource(this VersionedEventSource message)
        {
            return new Dolittle.Runtime.Events.VersionedEventSource(
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
        public static Dolittle.Runtime.Events.EventMetadata ToEventMetadata(this Protobuf.EventMetadata message)
        {
            var metadata = new Dolittle.Runtime.Events.EventMetadata(
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
        /// <param name="mapField"></param>
        /// <returns></returns>
        public static PropertyBag ToPropertyBag(this MapField<string, System.Protobuf.Object> mapField)
        {
            var dictionary = new Dictionary<string,object>();
            mapField.ForEach(keyValue => 
            {
                var type = (Types)keyValue.Value.Type;
                object value = null;

                using( var stream = new CodedInputStream(keyValue.Value.Content.ToByteArray()))
                {
                    switch( type )
                    {
                        case Types.String: value = stream.ReadString(); break;
                        case Types.Int32: value = stream.ReadInt32(); break;
                        case Types.Int64: value = stream.ReadInt64(); break;
                        case Types.UInt32: value = stream.ReadUInt32(); break;
                        case Types.UInt64: value = stream.ReadUInt64(); break;
                        case Types.Float: value = stream.ReadFloat(); break;
                        case Types.Double: value = stream.ReadDouble(); break;
                        case Types.Boolean: value = stream.ReadBool(); break;
                        case Types.DateTime: value = DateTime.FromFileTime(stream.ReadInt64()); break;
                        case Types.DateTimeOffset: value = DateTimeOffset.FromFileTime(stream.ReadInt64()); break;
                        case Types.Guid: value = new Guid(stream.ReadBytes().ToByteArray()); break;
                    }
                }
                dictionary.Add(keyValue.Key, value);
            });
            return new PropertyBag(dictionary);
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
            var versionedEventSource = new Dolittle.Runtime.Events.VersionedEventSource(eventSourceId, artifactId);
            var commitId = message.Id.ToConcept<CommitId>();
            var correlationId = message.CorrelationId.ToConcept<CorrelationId>();
            var timeStamp = DateTimeOffset.FromFileTime(message.TimeStamp);
            
            var events = message.Events.Select(_ =>                 
                new Dolittle.Runtime.Events.EventEnvelope(
                    _.Id.ToConcept<EventId>(),
                    _.Metadata.ToEventMetadata(),
                    _.Event.ToPropertyBag()
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