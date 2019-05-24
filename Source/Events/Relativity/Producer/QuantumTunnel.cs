/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Relativity.Protobuf.Conversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Grpc.Interaction;
using Dolittle.Serialization.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity {
    /// <summary>
    /// Represents an implementation of <see cref="IQuantumTunnel"/>
    /// </summary>
    public class QuantumTunnel : IQuantumTunnel {
        readonly ISerializer _serializer;
        readonly IServerStreamWriter<Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext> _responseStream;
        readonly ConcurrentQueue<Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext> _outbox;
        readonly ILogger _logger;

        readonly AutoResetEvent _waitHandle;
        readonly CancellationToken _cancelationToken;

        /// <summary>
        /// Initializes a new instance of <see cref="IQuantumTunnel"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use</param>
        /// <param name="responseStream">The committed event stream to pass through</param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public QuantumTunnel (
            ISerializer serializer,
            IServerStreamWriter<Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext> responseStream,
            CancellationToken cancellationToken,
            ILogger logger) {
            _responseStream = responseStream;
            _serializer = serializer;
            _outbox = new ConcurrentQueue<Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext> ();
            _waitHandle = new AutoResetEvent (false);
            _cancelationToken = cancellationToken;
            _logger = logger;
        }

        /// <inheritdoc/>
        public event QuantumTunnelCollapsed Collapsed = (q) => { };

        /// <inheritdoc/>
        public void PassThrough (Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext contextualEventStream) {
            try {
                var message = contextualEventStream.ToProtobuf ();
                _outbox.Enqueue (message);
                _waitHandle.Set ();
            } catch (Exception ex) {
                _logger.Error (ex, "Error creating and enqueueing committed event stream");
            }
        }

        /// <inheritdoc/>
        public async Task Open (IEnumerable<TenantOffset> tenantOffsets) {
            await Task.Run (async () => {
                for (;;) {
                    if (_cancelationToken.IsCancellationRequested) break;
                    try {
                        _waitHandle.WaitOne (1000);
                        if (_outbox.IsEmpty) continue;

                        Runtime.Grpc.Interaction.Protobuf.CommittedEventStreamWithContext message;
                        while (!_outbox.IsEmpty) {
                            if (_outbox.TryDequeue (out message)) {
                                try {
                                    await _responseStream.WriteAsync (message);
                                } catch (Exception ex) {
                                    _logger.Error (ex, "Error trying to send");
                                    break;
                                }
                            }
                        }
                    } catch (Exception outerException) {
                        _logger.Error (outerException, "Error during emptying of outbox");
                    }
                }
            });

            Collapsed (this);
        }

        void AddToQueue (IEnumerable<Commits> commits) {
            commits.ForEach (_ => AddToQueue (_));
        }

        void AddToQueue (Commits commits) {
            commits.ForEach (_ => AddToQueue (_));
        }

        void AddToQueue (Store.CommittedEventStream committedEventStream) {
            var originalContext = committedEventStream.Events.First ().Metadata.OriginalContext;
            _outbox.Enqueue (new Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext (committedEventStream, originalContext.ToExecutionContext (committedEventStream.CorrelationId)).ToProtobuf ());
        }
    }
}