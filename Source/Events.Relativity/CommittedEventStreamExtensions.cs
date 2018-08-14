/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using Dolittle.Collections;
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
        /// <param name="committedEventStream"></param>
        /// <param name="tenant"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
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
    }
}