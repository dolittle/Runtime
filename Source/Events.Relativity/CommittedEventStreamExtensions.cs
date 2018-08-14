/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Tenancy;
using Dolittle.Serialization.Protobuf;
using Google.Protobuf;

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
                Source = new VersionedEventSourceMessage
                {
                    Version = new EventSourceVersionMessage
                    {
                        Commit = committedEventStream.Source.Version.Commit,
                        Sequence = committedEventStream.Source.Version.Sequence,
                    },
                    EventSource = ByteString.CopyFrom(committedEventStream.Source.EventSource.Value.ToByteArray()),
                    Artifact = ByteString.CopyFrom(committedEventStream.Source.Artifact.Value.ToByteArray())
                },
                Sequence = committedEventStream.Sequence,
                Id = ByteString.CopyFrom(committedEventStream.Id.Value.ToByteArray()),
                TimeStamp = committedEventStream.Timestamp.ToFileTime(),
                CorrelationId = ByteString.CopyFrom(committedEventStream.CorrelationId.Value.ToByteArray()),
            };

            committedEventStream.Events.Select(@event => new EventParticleMessage
            {
                Id = ByteString.CopyFrom(@event.Id.Value.ToByteArray()),
                    Metadata = ByteString.CopyFrom(serializer.ToProtobuf(@event.Metadata)),
                    Event = ByteString.CopyFrom(serializer.ToProtobuf(@event.Metadata))
            }).ForEach(message.Events.Add);

            return message;
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

            /*
            var events = message.Events.Select(_ =>                 
                new Dolittle.Runtime.Events.Store.EventEnvelope(
                    (EventId)new Guid(_.Id.ToByteArray()),
                    

                )
            );
            */
            
            return new Dolittle.Runtime.Events.Store.CommittedEventStream(
                message.Sequence,
                versionedEventSource,
                commitId,
                correlationId,
                timeStamp,
                new EventStream(new Dolittle.Runtime.Events.Store.EventEnvelope[0])
            );
        }
    }
}