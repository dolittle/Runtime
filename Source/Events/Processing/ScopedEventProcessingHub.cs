namespace Dolittle.Runtime.Events.Processing
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Dolittle.Collections;
    using Dolittle.Execution;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Lifecycle;
    using System.Threading.Tasks;

    /// <summary>
    /// Tenant aware centalized Hub for processing events within the bounded context
    /// </summary>
    [Singleton]

    public class ScopedEventProcessingHub : IScopedEventProcessingHub
    {
        object _lockObj = new object();

        ConcurrentDictionary<ScopedEventProcessorKey, ConcurrentDictionary<EventProcessorId,ScopedEventProcessor>> _scopedProcessors 
                            = new ConcurrentDictionary<ScopedEventProcessorKey, ConcurrentDictionary<EventProcessorId,ScopedEventProcessor>>();

        BlockingCollection<CommittedEventStreamWithContext> _queue = new BlockingCollection<CommittedEventStreamWithContext>();

        System.Threading.ManualResetEvent _canProcessEvents = new System.Threading.ManualResetEvent(false);

        ILogger _logger;

        private readonly IExecutionContextManager _executionContextManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="executionContextManager"></param>
        public ScopedEventProcessingHub(ILogger logger, IExecutionContextManager executionContextManager)
        {
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        public void Register(ScopedEventProcessor processor)
        {
            _logger.Debug($"Adding ScopedEventProcessor  {processor.Key}");
            _scopedProcessors.GetOrAdd(processor.Key, (key) => new ConcurrentDictionary<EventProcessorId,ScopedEventProcessor>())
                .AddOrUpdate(processor.ProcessorId, processor, (k,v)=> processor);
            processor.CatchUp();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="committedEventStream"></param>
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
        

        /// <summary>
        /// Gets the <see cref="ScopedEventProcessor" /> registered for the specified key or null if none is registered
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>The registered <see cref="IEnumerable{ScopedEventProcessor}" /> for the key or null if none is registered</returns>
        public IEnumerable<ScopedEventProcessor> GetProcessorsFor(ScopedEventProcessorKey key)
        {
            ConcurrentDictionary<EventProcessorId,ScopedEventProcessor> processors;
            var dictionary = _scopedProcessors.TryGetValue(key, out processors) ? processors : null;
            return dictionary?.Values.ToArray() ?? Enumerable.Empty<ScopedEventProcessor>();
        }

        /// <summary>
        /// Adds the <see cref="CommittedEventStreamWithContext" /> to a queue for future processing
        /// </summary>
        /// <param name="committedEventStreamWithContext">the committed event stream with context to queue</param>
        protected virtual void Enqueue(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            lock(_lockObj)
            {
                _logger.Debug($"Enqueuing stream {committedEventStreamWithContext.EventStream.Sequence} {committedEventStreamWithContext.EventStream.Id} {committedEventStreamWithContext.EventStream.CorrelationId}");
                _queue.Add(committedEventStreamWithContext);
            }
        }

        /// <summary>
        ///  Processes the <see cref="CommittedEventStreamWithContext" /> 
        /// </summary>
        /// <param name="committedEventStreamWithContext">The <see cref="CommittedEventStream" /> to process</param>
        protected virtual void ProcessStream(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            _executionContextManager.CurrentFor(committedEventStreamWithContext.Context);
            var committedEventStream = committedEventStreamWithContext.EventStream;
            _logger.Debug($"Processing  stream {committedEventStream.Sequence} {committedEventStream.Id} {committedEventStream.CorrelationId}");
            committedEventStream.Events.ForEach(e => Process(e.ToCommittedEventEnvelope(committedEventStream.Sequence),committedEventStreamWithContext.Context));
        }

        void Process(CommittedEventEnvelope envelope, ExecutionContext executionContext)
        {        
            ConcurrentDictionary<EventProcessorId,ScopedEventProcessor> processors = null;
            var key = new ScopedEventProcessorKey(executionContext.Tenant,envelope.Metadata.Artifact);
            if (_scopedProcessors.TryGetValue(key, out processors))
            {
                if(processors.Values.Any())
                {
                    processors.Values.ForEach(_ => _.Process(envelope));
                }
                //Parallel.ForEach(processors.Values, _ => _.Process(envelope));
            }
            if (processors.Count == 0)
                _logger.Warning($"No Processor registered for {key}");
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginProcessingEvents()
        {
            _logger.Debug($"BeginProcessingEvents");
            ProcessQueue();
        }

        void ProcessQueue()
        {
            lock(_lockObj)
            {
                _logger.Debug($"Processing committed event stream queue");
                _queue.CompleteAdding();
                if(_queue.Count == 0)
                {
                    _canProcessEvents.Set();// = true;
                    return;
                }
                foreach (CommittedEventStreamWithContext contextualStream in _queue.GetConsumingEnumerable().ToList())
                {
                    ProcessStream(contextualStream);
                }
                _canProcessEvents.Set();// = true;
                _logger.Debug($"Processed committed event stream queue");
            }
        }
    }
}