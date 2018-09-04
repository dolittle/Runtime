namespace Dolittle.Runtime.Events.Processing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dolittle.Bootstrapping;
    using Dolittle.Collections;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Tenancy;
    using Dolittle.Types;

    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="EventProcessors"/>
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        IInstancesOf<IKnowAboutEventProcessors> _systemsThatKnowAboutEventProcessors;
        ITenants _tenants;

        IScopedEventProcessingHub _processingHub;
        ILogger _logger;
        private readonly Func<IEventProcessorOffsetRepository> _getOffsetRepository;
        private readonly Func<IFetchUnprocessedEvents> _getUnprocessedEventsFetcher;

        /// <summary>
        /// Instantiates a new instance of <see cref="BootProcedure" />
        /// </summary>
        /// <param name="systemsThatKnowAboutEventProcessors">Provides <see cref="IEventProcessor">Event Processors</see></param>
        /// <param name="tenants">A collection of all <see cref="ITenant">tenants</see></param>
        /// <param name="processingHub">An instance of <see cref="IScopedEventProcessingHub" /> for processing <see cref="CommittedEventStream">Committed Event Streams</see></param>
        /// <param name="getOffsetRepository">A factory function to return a correctly scoped instance of <see cref="IEventProcessorOffsetRepository" /></param>
        /// <param name="getUnprocessedEventsFetcher">A factory function to return a correctly scoped instance of <see cref="IFetchUnprocessedEvents" /></param>
        /// <param name="logger">An instance of <see cref="ILogger" /> for logging</param>
        public BootProcedure(IInstancesOf<IKnowAboutEventProcessors> systemsThatKnowAboutEventProcessors, 
                                ITenants tenants, 
                                IScopedEventProcessingHub processingHub, 
                                Func<IEventProcessorOffsetRepository> getOffsetRepository, 
                                Func<IFetchUnprocessedEvents> getUnprocessedEventsFetcher, 
                                ILogger logger)
        {
            _processingHub = processingHub;
            _logger = logger;
            _tenants = tenants;
            _systemsThatKnowAboutEventProcessors = systemsThatKnowAboutEventProcessors;
            _getOffsetRepository = getOffsetRepository;
            _getUnprocessedEventsFetcher = getUnprocessedEventsFetcher;
        }

        /// <inheritdoc />
        public bool CanPerform() => true;

        /// <inheritdoc />
        public void Perform()
        {
            ProcessInParallel();
            _processingHub.BeginProcessingEvents();
        }

        void ProcessInParallel()
        {
            Parallel.ForEach(_systemsThatKnowAboutEventProcessors.ToList(), (system) =>
            {
                Parallel.ForEach(system.ToList(), (processor) =>
                {
                    Parallel.ForEach(_tenants.ToList(), (t) =>
                    {
                        _processingHub.Register(new ScopedEventProcessor(t, processor,_getOffsetRepository,_getUnprocessedEventsFetcher, _logger));
                    });
                });
            });
        }
    }
}