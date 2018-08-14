/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Logging;
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
            readonly ConcurrentQueue<CommittedEventStreamParticleMessage> _outbox;
            readonly ILogger _logger;

            readonly AutoResetEvent _waitHandle;

            /// <summary>
            /// Initializes a new instance of <see cref="IQuantumTunnel"/>
            /// </summary>
            /// <param name="serializer"><see cref="ISerializer"/> to use</param>
            /// <param name="responseStream">The <see cref="IServerStreamWriter{EventParticleMessage}"/> to pass through</param>
            /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to get current context from</param>
            /// <param name="logger"><see cref="ILogger"/> for logging</param>
            public QuantumTunnel(
                ISerializer serializer,
                IServerStreamWriter<CommittedEventStreamParticleMessage> responseStream,
                IExecutionContextManager executionContextManager,
                ILogger logger)
            {
                _responseStream = responseStream;
                _serializer = serializer;
                _executionContextManager = executionContextManager;
                _outbox = new ConcurrentQueue<CommittedEventStreamParticleMessage>();
                _logger = logger;
                _waitHandle = new AutoResetEvent(false);
            }

            /// <inheritdoc/>
            public event QuantumTunnelCollapsed Collapsed = (q) => { };

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

                    _outbox.Enqueue(stream);
                    _waitHandle.Set();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error creating and enqueueing committed event stream");
                }
            }

            /// <inheritdoc/>
            public async Task Open()
            {
                await Task.Run(() =>
                    {
                        for (;;)
                        {
                            try
                            {
                                _waitHandle.WaitOne(1000);
                                if (_outbox.IsEmpty) continue;

                                CommittedEventStreamParticleMessage message;
                                if (_outbox.TryDequeue(out message))
                                {
                                    try
                                    {
                                        _responseStream.WriteAsync(message);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error(ex, "Error trying to send");
                                        break;
                                    }
                                }
                            }
                            catch (Exception outerException)
                            {
                                _logger.Error(outerException, "Error during emptying of outbox");
                            }
                        }
                    });

                    Collapsed(this);
                }
            }
        }