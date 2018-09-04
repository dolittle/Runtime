namespace Dolittle.Runtime.Events.Processing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dolittle.Bootstrapping;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Tenancy;
    using Dolittle.Types;

    /// <summary>
    /// 
    /// </summary>
    public class ScopedEventProcessor
    {
        CommittedEventVersion _lastVersionProcessed;

        public ScopedEventProcessorKey Key { get; }
        public EventProcessorIdentifier ProcessorId => _processor?.Identifier;

        TenantId _tenant;
        IEventProcessor _processor;
        ILogger _logger;

        public ScopedEventProcessor(TenantId tenant, IEventProcessor processor, ILogger logger)
        {
            _tenant = tenant;
            _processor = processor;
            _logger = logger;
            Key = new ScopedEventProcessorKey(tenant,processor.Event);
        }

        public virtual void CatchUp()
        {
            //SetIsCaughtUp(committedEventVersion);
        }

        private void SetIsCaughtUp(CommittedEventVersion committedEventVersion)
        {
            _lastVersionProcessed = committedEventVersion;
            HasCaughtUp = true;
        }

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
        public bool HasCaughtUp { get; private set; }

        public bool CanProcess(CommittedEventVersion version)
        {
            return HasCaughtUp && version > _lastVersionProcessed;
        }
    }
}