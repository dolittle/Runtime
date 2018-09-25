/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Serialization.Protobuf;
using Dolittle.Runtime.Events.Relativity.Protobuf;
using Grpc.Core;
using Dolittle.Execution;
using Dolittle.Runtime.Events.Store;
using Dolittle.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IQuantumTunnel"/>
    /// </summary>
    public class QuantumTunnel : IQuantumTunnel
    {
        readonly ISerializer _serializer;
        readonly IServerStreamWriter<Protobuf.CommittedEventStreamWithContext> _responseStream;
        readonly IExecutionContextManager _executionContextManager;
        readonly ConcurrentQueue<Protobuf.CommittedEventStreamWithContext> _outbox;
        readonly ILogger _logger;

        readonly AutoResetEvent _waitHandle;
        private readonly IFetchUnprocessedCommits _unprocessedCommitFetcher;

        /// <summary>
        /// Initializes a new instance of <see cref="IQuantumTunnel"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use</param>
        /// <param name="responseStream">The committed event stream to pass through</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to get current context from</param>
        /// <param name="unprocessedCommitFetcher">An <see cref="IFetchUnprocessedCommits" /> to fetch unprocessed commits</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public QuantumTunnel(
            ISerializer serializer,
            IServerStreamWriter<Protobuf.CommittedEventStreamWithContext> responseStream,
            IExecutionContextManager executionContextManager,
            IFetchUnprocessedCommits unprocessedCommitFetcher,
            ILogger logger)
        {
            _responseStream = responseStream;
            _serializer = serializer;
            _executionContextManager = executionContextManager;
            _outbox = new ConcurrentQueue<Protobuf.CommittedEventStreamWithContext>();
            _logger = logger;
            _waitHandle = new AutoResetEvent(false);
            _unprocessedCommitFetcher = unprocessedCommitFetcher;
        }

        /// <inheritdoc/>
        public event QuantumTunnelCollapsed Collapsed = (q) => { };

        /// <inheritdoc/>
        public void PassThrough(Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext contextualEventStream)
        {
            try
            {
                var message = contextualEventStream.ToMessage();
                _outbox.Enqueue(message);
                _waitHandle.Set();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating and enqueueing committed event stream");
            }
        }

        /// <inheritdoc/>
        public async Task Open(IEnumerable<TenantOffset> tenantOffsets)
        {
            FetchAndQueueCommits(tenantOffsets);
            await Task.Run(async() =>
            {
                for (;;)
                {
                    try
                    {
                        _waitHandle.WaitOne(1000);
                        if (_outbox.IsEmpty) continue;

                        Protobuf.CommittedEventStreamWithContext message;
                        while (!_outbox.IsEmpty)
                        {
                            if (_outbox.TryDequeue(out message))
                            {
                                try
                                {
                                    await _responseStream.WriteAsync(message);
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
            });

            Collapsed(this);
        }

        void FetchAndQueueCommits(IEnumerable<TenantOffset> tenantOffsets)
        {
            var commits = GetCommits(tenantOffsets);
            AddToQueue(commits);
        }

        IEnumerable<Commits> GetCommits(IEnumerable<TenantOffset> tenantOffsets)
        {
            List<Commits> commits = new List<Commits>();
            Parallel.ForEach(tenantOffsets,(_) => {
                commits.Add(_unprocessedCommitFetcher.GetUnprocessedCommits(_.Offset));
            });
            return commits;
        }

        void AddToQueue(IEnumerable<Commits> commits)
        {
            commits.ForEach(_ => AddToQueue(_));
        }

        void AddToQueue(Commits commits)
        {
            commits.ForEach(_ => AddToQueue(_));
        }

        void AddToQueue(Store.CommittedEventStream committedEventStream)
        {
            var originalContext = committedEventStream.Events.First().Metadata.OriginalContext;
            _outbox.Enqueue(new Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext(committedEventStream,originalContext.ToExecutionContext(committedEventStream.CorrelationId)).ToMessage());
        }
    }
}