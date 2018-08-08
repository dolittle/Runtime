/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Serialization.Protobuf;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IQuantumTunnel"/>
    /// </summary>
    public class QuantumTunnel : IQuantumTunnel
    {
        readonly ISerializer _serializer;
        readonly IServerStreamWriter<CommittedEventStreamParticleMessage> _responseStream;

        /// <summary>
        /// Initializes a new instance of <see cref="IQuantumTunnel"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use</param>
        /// <param name="responseStream">The <see cref="IServerStreamWriter{EventParticleMessage}"/> to pass through</param>
        public QuantumTunnel(ISerializer serializer, IServerStreamWriter<CommittedEventStreamParticleMessage> responseStream)
        {
            _responseStream = responseStream;
            _serializer = serializer;
        }
        
        /// <inheritdoc/>
        public event QuantumTunnelCollapsed Collapsed = (q) => {};

        /// <inheritdoc/>
        public void PassThrough(Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream)
        {
            try 
            {
                var stream = new CommittedEventStreamParticleMessage
                {
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
                    Metadata = ByteString.CopyFrom(_serializer.ToProtobuf(@event.Metadata)),
                    Event = ByteString.CopyFrom(_serializer.ToProtobuf(@event.Metadata))
                }).ForEach(stream.Events.Add);

                _responseStream.WriteAsync(stream);
            } 
            catch 
            {
                Collapsed(this);
            }
        }
    }
}