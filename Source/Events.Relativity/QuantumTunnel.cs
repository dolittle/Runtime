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
                var message = committedEventStream.ToMessage(_executionContextManager.Current.Tenant.TenantId, _serializer);
                _outbox.Enqueue(message);
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