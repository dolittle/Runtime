namespace Dolittle.Runtime.Events.Processing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dolittle.Artifacts;
    using Dolittle.Bootstrapping;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Tenancy;
    using Dolittle.Types;

    /// <summary>
    /// Processes an individiual <see cref="CommittedEventEnvelope" /> for the correct <see cref="TenantId" />
    /// </summary>
    public class ScopedEventProcessor
    {
        CommittedEventVersion _lastVersionProcessed;

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

        /// <summary>
        /// Instantiates an instance of <see cref="ScopedEventProcessor" />
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" /> that this processor is scoped to.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages</param>
        public ScopedEventProcessor(TenantId tenant, IEventProcessor processor, ILogger logger)
        {
            _tenant = tenant;
            _processor = processor;
            _logger = logger;
            Key = new ScopedEventProcessorKey(tenant,processor.Event);
        }

        /// <summary>
        /// Instructs the Processor to Catch up by processing events that have been committed since the last processed event
        /// </summary>
        public virtual void CatchUp()
        {
            //SetIsCaughtUp(committedEventVersion);
        }

        private void SetIsCaughtUp(CommittedEventVersion committedEventVersion)
        {
            _lastVersionProcessed = committedEventVersion;
            HasCaughtUp = true;
        }

        /// <summary>
        /// Processes a single <see cref="CommittedEventEnvelope" />
        /// </summary>
        /// <param name="envelope">The <see cref="CommittedEventEnvelope" />to process</param>
        public virtual void Process(CommittedEventEnvelope envelope)
        {
            if(CanProcess(envelope.Version))
                ProcessEvent(envelope);
        }

        void ProcessEvent(CommittedEventEnvelope envelope)
        {
            _lastVersionProcessed = envelope.Version;
            _logger.Debug($"Processing {envelope.Version}");
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
            return HasCaughtUp && version > _lastVersionProcessed;
        }
    }
}