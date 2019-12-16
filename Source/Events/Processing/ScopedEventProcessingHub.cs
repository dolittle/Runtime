// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IScopedEventProcessingHub"/>.
    /// </summary>
    [Singleton]

    public class ScopedEventProcessingHub : IScopedEventProcessingHub, IDisposable
    {
        readonly object _lockObj = new object();

        readonly ConcurrentDictionary<ScopedEventProcessorKey, ConcurrentDictionary<EventProcessorId, ScopedEventProcessor>> _scopedProcessors
                            = new ConcurrentDictionary<ScopedEventProcessorKey, ConcurrentDictionary<EventProcessorId, ScopedEventProcessor>>();

        readonly BlockingCollection<CommittedEventStreamWithContext> _queue = new BlockingCollection<CommittedEventStreamWithContext>();

        readonly System.Threading.ManualResetEvent _canProcessEvents = new System.Threading.ManualResetEvent(false);

        readonly ILogger _logger;

        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedEventProcessingHub"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with <see cref="ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public ScopedEventProcessingHub(
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Register(ScopedEventProcessor processor)
        {
            _logger.Debug($"Adding ScopedEventProcessor  {processor.Key}");
            _scopedProcessors.GetOrAdd(processor.Key, (_) => new ConcurrentDictionary<EventProcessorId, ScopedEventProcessor>())
                .AddOrUpdate(processor.ProcessorId, processor, (_, __) => processor);
            processor.CatchUp();
        }

        /// <inheritdoc/>
        public void Process(CommittedEventStream committedEventStream)
        {
            var committedEventStreamWithContext = new CommittedEventStreamWithContext(committedEventStream, _executionContextManager.Current);
            _logger.Debug($"Received  stream {committedEventStream.Sequence} {committedEventStream.Id} {committedEventStream.CorrelationId}");
            if (_canProcessEvents.WaitOne(TimeSpan.FromMilliseconds(1)))
            {
                _logger.Debug($"Processing  stream {committedEventStream.Sequence} {committedEventStream.Id} {committedEventStream.CorrelationId}");
                ProcessStream(committedEventStreamWithContext);
            }
            else
            {
                _logger.Debug($"Queuing  stream {committedEventStream.Sequence} {committedEventStream.Id} {committedEventStream.CorrelationId}");
                Enqueue(committedEventStreamWithContext);
            }
        }

        /// <inheritdoc/>
        public void BeginProcessingEvents()
        {
            _logger.Debug($"BeginProcessingEvents");
            ProcessQueue();
        }

        /// <summary>
        /// Gets the <see cref="ScopedEventProcessor" /> registered for the specified key or null if none is registered.
        /// </summary>
        /// <param name="key">Key to search for.</param>
        /// <returns>The registered <see cref="IEnumerable{ScopedEventProcessor}" /> for the key or null if none is registered.</returns>
        public IEnumerable<ScopedEventProcessor> GetProcessorsFor(ScopedEventProcessorKey key)
        {
            var dictionary = _scopedProcessors.TryGetValue(key, out ConcurrentDictionary<EventProcessorId, ScopedEventProcessor> processors) ? processors : null;
            return dictionary?.Values.ToArray() ?? Enumerable.Empty<ScopedEventProcessor>();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _canProcessEvents.Dispose();
            _queue.Dispose();
        }

        /// <summary>
        /// Adds the <see cref="CommittedEventStreamWithContext" /> to a queue for future processing.
        /// </summary>
        /// <param name="committedEventStreamWithContext">the committed event stream with context to queue.</param>
        protected virtual void Enqueue(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            lock (_lockObj)
            {
                _logger.Debug($"Enqueuing stream {committedEventStreamWithContext.EventStream.Sequence} {committedEventStreamWithContext.EventStream.Id} {committedEventStreamWithContext.EventStream.CorrelationId}");
                _queue.Add(committedEventStreamWithContext);
            }
        }

        /// <summary>
        ///  Processes the <see cref="CommittedEventStreamWithContext" />.
        /// </summary>
        /// <param name="committedEventStreamWithContext">The <see cref="CommittedEventStream" /> to process.</param>
        protected virtual void ProcessStream(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            _executionContextManager.CurrentFor(committedEventStreamWithContext.Context);
            var committedEventStream = committedEventStreamWithContext.EventStream;
            _logger.Debug($"Processing  stream {committedEventStream.Sequence} {committedEventStream.Id} {committedEventStream.CorrelationId}");
            committedEventStream.Events.ForEach(e => Process(e.ToCommittedEventEnvelope(committedEventStream.Sequence), committedEventStreamWithContext.Context));
        }

        void Process(CommittedEventEnvelope envelope, ExecutionContext executionContext)
        {
            var key = new ScopedEventProcessorKey(executionContext.Tenant, envelope.Metadata.Artifact);
            if (_scopedProcessors.TryGetValue(key, out ConcurrentDictionary<EventProcessorId, ScopedEventProcessor> processors))
            {
                if (processors?.Values.Any() ?? false)
                {
                    processors.Values.ForEach(_ => _.Process(envelope));
                }
            }

            if (processors == null || processors.Count == 0)
                _logger.Warning($"No Processor registered for {key}");
        }

        void ProcessQueue()
        {
            lock (_lockObj)
            {
                _logger.Debug($"Processing committed event stream queue");
                _queue.CompleteAdding();
                if (_queue.Count == 0)
                {
                    _canProcessEvents.Set();
                    return;
                }

                foreach (CommittedEventStreamWithContext contextualStream in _queue.GetConsumingEnumerable().ToList())
                {
                    ProcessStream(contextualStream);
                }

                _canProcessEvents.Set();
                _logger.Debug($"Processed committed event stream queue");
            }
        }
    }
}