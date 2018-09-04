namespace Dolittle.Runtime.Events.Processing
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System;
    using Dolittle.Applications;
    using Dolittle.Collections;
    using Dolittle.Execution;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Execution;
    using Dolittle.Runtime.Tenancy;

    /// <summary>
    /// 
    /// </summary>
    [Singleton]

    public class ScopedEventProcessingHub : IScopedEventProcessingHub
    {
        object _lockObj = new object();

        ConcurrentDictionary<ScopedEventProcessorKey, ConcurrentDictionary<EventProcessorIdentifier,ScopedEventProcessor>> _scopedProcessors 
                            = new ConcurrentDictionary<ScopedEventProcessorKey, ConcurrentDictionary<EventProcessorIdentifier,ScopedEventProcessor>>();

        BlockingCollection<CommittedEventStreamWithContext> _queue = new BlockingCollection<CommittedEventStreamWithContext>();

        ManualResetEvent _canProcessEvents = new ManualResetEvent(false);

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
            _scopedProcessors.GetOrAdd(processor.Key, (key) => new ConcurrentDictionary<EventProcessorIdentifier,ScopedEventProcessor>())
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
            ConcurrentDictionary<EventProcessorIdentifier,ScopedEventProcessor> processors;
            var dictionary = _scopedProcessors.TryGetValue(key, out processors) ? processors : null;
            return dictionary?.Values.ToArray() ?? Enumerable.Empty<ScopedEventProcessor>();
        }

        protected virtual void Enqueue(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            lock(_lockObj)
            {
                _logger.Debug($"Enqueuing stream {committedEventStreamWithContext.EventStream.Sequence} {committedEventStreamWithContext.EventStream.Id} {committedEventStreamWithContext.EventStream.CorrelationId}");
                _queue.Add(committedEventStreamWithContext);
            }
        }

        protected virtual void ProcessStream(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            var committedEventStream = committedEventStreamWithContext.EventStream;
            _logger.Debug($"Processing  stream {committedEventStream.Sequence} {committedEventStream.Id} {committedEventStream.CorrelationId}");
            committedEventStream.Events.ForEach(e => Process(e.ToCommittedEventEnvelope(committedEventStream.Sequence),committedEventStreamWithContext.Context));
        }

        void Process(CommittedEventEnvelope envelope, IExecutionContext executionContext)
        {
            ConcurrentDictionary<EventProcessorIdentifier,ScopedEventProcessor> processors = null;
            var key = new ScopedEventProcessorKey(executionContext.Tenant,envelope.Metadata.Artifact);
            if (_scopedProcessors.TryGetValue(key, out processors))
            {
                processors.Values.ForEach(_ => _.Process(envelope));
            }
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

    public class CommittedEventStreamWithContext
    {
        public CommittedEventStream EventStream { get; }
        public IExecutionContext Context { get; }

        public CommittedEventStreamWithContext(CommittedEventStream stream, IExecutionContext context)
        {
            EventStream = stream;
            Context = context;
        }
    }    

    public interface IExecutionContextManager
    {
        IExecutionContext Current { get; set; }

        void SetConstants(Application application, BoundedContext boundedContext);

        IExecutionContext CurrentFor(TenantId tenant);

        IExecutionContext CurrentFor(TenantId tenant, CorrelationId correlationId, ClaimsPrincipal principal = null);
    }

    public class ExecutionContextManager : IExecutionContextManager
    {
        static AsyncLocal<IExecutionContext> _executionContext = new AsyncLocal<IExecutionContext>();

        Application _application;
        BoundedContext _boundedContext;

        /// <inheritdoc/>
        public IExecutionContext Current
        {
            get
            {
                var context = _executionContext.Value;
                if (context == null)throw new Exception("Execution Context not set");
                return context;
            }
            set { _executionContext.Value = value; }
        }

        /// <inheritdoc/>
        public void SetConstants(Application application, BoundedContext boundedContext)
        {
            _application = application;
            _boundedContext = boundedContext;
        }

        /// <inheritdoc/>
        public IExecutionContext CurrentFor(TenantId tenant)
        {
            return CurrentFor(tenant, CorrelationId.New(), new ClaimsPrincipal());
        }

        /// <inheritdoc/>
        public IExecutionContext CurrentFor(TenantId tenant, CorrelationId correlationId, ClaimsPrincipal principal = null)
        {
            var executionContext = new ExecutionContext(
                _application,
                _boundedContext,
                tenant,
                correlationId,
                principal ?? new ClaimsPrincipal(),
                CultureInfo.CurrentCulture);

            Current = executionContext;

            return executionContext;
        }
    }

    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(
            Application application,
            BoundedContext boundedContext,
            TenantId tenant,
            CorrelationId correlationId,
            ClaimsPrincipal principal,
            CultureInfo cultureInfo)
        {
            Application = application;
            BoundedContext = boundedContext;
            Tenant = tenant;
            CorrelationId = correlationId;
            Principal = principal;
            Culture = cultureInfo;
        }

        /// <inheritdoc/>
        public Application Application { get; }

        /// <inheritdoc/>
        public BoundedContext BoundedContext { get; }

        /// <inheritdoc/>
        public TenantId Tenant { get; }

        /// <inheritdoc/>
        public CorrelationId CorrelationId { get; }

        /// <inheritdoc/>
        public ClaimsPrincipal Principal { get; }

        /// <inheritdoc/>
        public CultureInfo Culture { get; }
    }

    public interface IExecutionContext
    {
        /// <summary>
        /// Gets the <see cref="Application"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        Application Application { get; }

        /// <summary>
        /// Gets the <see cref="BoundedContext"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        BoundedContext BoundedContext { get; }

        /// <summary>
        /// Gets the <see cref="TenantId"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        TenantId Tenant { get; }

        /// <summary>
        /// Gets the <see cref="CorrelationId"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        ClaimsPrincipal Principal { get; }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> for the <see cref="IExecutionContext">execution context</see>
        /// </summary>
        CultureInfo Culture { get; }
    }
}