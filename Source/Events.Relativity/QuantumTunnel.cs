/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Runtime.Execution;
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
        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of <see cref="IQuantumTunnel"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use</param>
        /// <param name="responseStream">The <see cref="IServerStreamWriter{EventParticleMessage}"/> to pass through</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to get current context from</param>
        public QuantumTunnel(
            ISerializer serializer,
            IServerStreamWriter<CommittedEventStreamParticleMessage> responseStream,
            IExecutionContextManager executionContextManager)
        {
            _responseStream = responseStream;
            _serializer = serializer;
            _executionContextManager = executionContextManager;
        }
        
        /// <inheritdoc/>
        public event QuantumTunnelCollapsed Collapsed = (q) => {};

        /// <inheritdoc/>
        public void PassThrough(Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream)
        {
            try 
            {
                var executionContext = _executionContextManager.Current;

                var stream = new CommittedEventStreamParticleMessage
                {
                    Tenant = ByteString.CopyFrom(executionContext.Tenant.TenantId.Value.ToByteArray()),
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

        /// <inheritdoc/>
        public async Task Open()
        {
            await Task.Run(() => {
                for( ;; )
                {
                    try {

                    } catch {
                        break;
                    }
                    Thread.Sleep(1000);
                }
            });

            Collapsed(this);
        }
    }
}