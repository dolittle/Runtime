// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Relativity.Protobuf.Conversion;
using Dolittle.Serialization.Protobuf;
using Grpc.Core;
using grpc = Dolittle.Events.Relativity.Microservice;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IQuantumTunnel"/>.
    /// </summary>
    public class QuantumTunnel : IQuantumTunnel, IDisposable
    {
        readonly ISerializer _serializer;
        readonly IServerStreamWriter<grpc.CommittedEventStreamWithContext> _responseStream;
        readonly ConcurrentQueue<grpc.CommittedEventStreamWithContext> _outbox;
        readonly ILogger _logger;
        readonly AutoResetEvent _waitHandle;
        readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuantumTunnel"/> class.
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use.</param>
        /// <param name="responseStream">The committed event stream to pass through.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to use.</param>
        public QuantumTunnel(
            ISerializer serializer,
            IServerStreamWriter<grpc.CommittedEventStreamWithContext> responseStream,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            _responseStream = responseStream;
            _serializer = serializer;
            _outbox = new ConcurrentQueue<grpc.CommittedEventStreamWithContext>();
            _waitHandle = new AutoResetEvent(false);
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        /// <inheritdoc/>
        public event QuantumTunnelCollapsed Collapsed = (_) => { };

        /// <inheritdoc/>
        public void Dispose()
        {
            _waitHandle.Dispose();
        }

        /// <inheritdoc/>
        public void PassThrough(Processing.CommittedEventStreamWithContext contextualEventStream)
        {
            try
            {
                var message = contextualEventStream.ToProtobuf();
                _outbox.Enqueue(message);
                _waitHandle.Set();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating and enqueueing committed event stream");
            }
        }

        /// <summary>
        /// Opens the tunnel.
        /// </summary>
        /// <param name="tenantOffsets"><see cref="IEnumerable{T}"/> of <see cref="TenantOffset"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Open(IEnumerable<TenantOffset> tenantOffsets)
        {
            if (tenantOffsets == null) return;

            await Task.Run(async () =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        _waitHandle.WaitOne(1000);
                        if (_outbox.IsEmpty) continue;

                        while (!_outbox.IsEmpty)
                        {
                            if (_outbox.TryDequeue(out grpc.CommittedEventStreamWithContext message))
                            {
                                try
                                {
                                    await _responseStream.WriteAsync(message).ConfigureAwait(false);
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(ex, "Error trying to send");
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception outerException)
                    {
                        _logger.Error(outerException, "Error during emptying of outbox");
                    }
                }
            }).ConfigureAwait(false);

            Collapsed(this);
        }
    }
}
