namespace Dolittle.Runtime.Events.Processing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dolittle.Artifacts;
    using Dolittle.Bootstrapping;
    using Dolittle.Collections;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Tenancy;
    using Dolittle.Types;
    using Dolittle.Tenancy;
    using Dolittle.DependencyInversion;

    /// <summary>
    /// Processes an individiual <see cref="CommittedEventEnvelope" /> for the correct <see cref="TenantId" />
    /// </summary>
    public class ScopedEventProcessor
    {
        object lockObject = new object();

        /// <summary>
        /// Indicates the <see cref="CommittedEventVersion">version</see> of the last event that was processed
        /// </summary>
        /// <value></value>      
        public CommittedEventVersion LastVersionProcessed { get; private set; }

        /// <summary>
        /// A <see cref="ScopedEventProcessorKey" /> to identity the <see cref="Artifact">Event</see> and <see cref="TenantId">Tenant</see> combination
        /// </summary>
        /// <value></value>
        public ScopedEventProcessorKey Key { get; }
        /// <summary>
        /// Unique identifer for the <see cref="IEventProcessor" />
        /// </summary>
        public EventProcessorId ProcessorId => _processor?.Identifier;

        TenantId _tenant;
        IEventProcessor _processor;
        ILogger _logger;
        private readonly FactoryFor<IFetchUnprocessedEvents> _getUnprocessedEventsFetcher;
        private readonly FactoryFor<IEventProcessorOffsetRepository> _getOffsetRepository;

        /// <summary>
        /// Instantiates an instance of <see cref="ScopedEventProcessor" />
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" /> that this processor is scoped to.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event</param>
        /// <param name="getOffsetRepository">A factory function to return a correctly scoped instance of <see cref="IEventProcessorOffsetRepository" /></param>
        /// <param name="getUnprocessedEventsFetcher">A factory function to return a correctly scoped instance of <see cref="IFetchUnprocessedEvents" /></param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages</param>
        public ScopedEventProcessor(TenantId tenant, IEventProcessor processor,FactoryFor<IEventProcessorOffsetRepository> getOffsetRepository, 
                                        FactoryFor<IFetchUnprocessedEvents> getUnprocessedEventsFetcher,  ILogger logger)
        {
            LastVersionProcessed = CommittedEventVersion.None;
            _tenant = tenant;
            _processor = processor;
            _logger = logger;
            Key = new ScopedEventProcessorKey(tenant,processor.Event);
            _getUnprocessedEventsFetcher = getUnprocessedEventsFetcher;
            _getOffsetRepository = getOffsetRepository;
        }

        /// <summary>
        /// Instructs the Processor to Catch up by processing events that have been committed since the last processed event
        /// </summary>
        public virtual void CatchUp()
        {
            using(var repository = _getOffsetRepository())
            {
                LastVersionProcessed = _getOffsetRepository().Get(ProcessorId);
            }

            SingleEventTypeEventStream eventStream = _getUnprocessedEventsFetcher().GetUnprocessedEvents(Key.Event.Id,LastVersionProcessed) ?? new SingleEventTypeEventStream(null);
            do
            {
                if(!eventStream.IsEmpty)
                {
                    eventStream.ForEach(e => ProcessEvent(e));
                    eventStream = _getUnprocessedEventsFetcher().GetUnprocessedEvents(Key.Event.Id,LastVersionProcessed);
                }
            }
            while(!eventStream.IsEmpty);
            HasCaughtUp = true;
        }

        /// <summary>
        /// Processes a single <see cref="CommittedEventEnvelope" />
        /// </summary>
        /// <param name="envelope">The <see cref="CommittedEventEnvelope" />to process</param>
        public virtual void Process(CommittedEventEnvelope envelope)
        {
            lock(lockObject)
            {
                Console.WriteLine($"Event Processor: {_processor.Identifier.Value} Checking if can process: {envelope.Version.Major}.{envelope.Version.Minor}.{envelope.Version.Revision} - {envelope.Metadata.Artifact.Id} : {CanProcess(envelope.Version)}");
                if(CanProcess(envelope.Version))
                {
                    Console.WriteLine($"Event Processor: {_processor.Identifier.Value} Processing: {envelope.Version.Major}.{envelope.Version.Minor}.{envelope.Version.Revision} - {envelope.Metadata.Artifact.Id}");
                    ProcessEvent(envelope);
                } 
            }  
        }

        void ProcessEvent(CommittedEventEnvelope envelope)
        { 
            lock(lockObject)
            {
                _logger.Debug($"Processing {envelope.Version.ToString()} of {Key.Event.Id}");
                var previousVersion = LastVersionProcessed;
                try
                {
                    SetCurrentVersion(envelope.Version);
                    _processor.Process(envelope);
                    _logger.Debug($"Processed {envelope.Version.ToString()} of {Key.Event.Id}");
                }
                catch(Exception ex)
                {
                    _logger.Error($"Error Processing {envelope.Version.ToString()} of {Key.Event.Id}.  Resetting to previous version. {ex.Message}");
                    SetCurrentVersion(previousVersion);
                    throw;
                }
            }
            _logger.Debug($"Processing {envelope.Version.ToString()} of {Key.Event.Id}");
        }

        void SetCurrentVersion(CommittedEventVersion committedEventVersion)
        {
            //the provider should accept the version and is responsible for transient errors and persisting it eventually
            try
            {
                LastVersionProcessed = committedEventVersion;
                using(var repository = _getOffsetRepository())
                {
                    repository.Set(ProcessorId,LastVersionProcessed);
                } 
            }
            catch(Exception ex)
            {
                _logger.Error($"Error setting offset for '{ProcessorId}' : '{committedEventVersion}' - {ex.ToString()}");
            }
        }

        /// <summary>
        /// Indicates whether the processor has caught up with committed events
        /// </summary>
        /// <value></value>
        public bool HasCaughtUp { get; private set; }

        /// <summary>
        /// Indicates whether the processor can process the <see cref="CommittedEventVersion" /> provided.
        /// </summary>
        /// <param name="version"></param>
        /// <returns>True if the processor has caught up and the version is greater than the last processed version, otherwise false</returns>
        public bool CanProcess(CommittedEventVersion version)
        {
            return HasCaughtUp && version > LastVersionProcessed;
        }
    }
}