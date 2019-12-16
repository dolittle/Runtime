// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Applications;
using Dolittle.Booting;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.ResourceTypes.Configuration;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Tenancy;
using Dolittle.Scheduling;
using Dolittle.Security;
using Dolittle.Types;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="EventProcessors"/>.
    /// </summary>
    public class BootProcedure : ICanPerformBootProcedure
    {
        readonly IInstancesOf<IKnowAboutEventProcessors> _systemsThatKnowAboutEventProcessors;
        readonly ITenants _tenants;
        readonly IScopedEventProcessingHub _processingHub;
        readonly ILogger _logger;
        readonly FactoryFor<IEventProcessorOffsetRepository> _getOffsetRepository;
        readonly FactoryFor<IFetchUnprocessedEvents> _getUnprocessedEventsFetcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly IResourceConfiguration _resourceConfiguration;
        readonly IScheduler _scheduler;
        int _canPerformCount = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootProcedure"/> class.
        /// </summary>
        /// <param name="systemsThatKnowAboutEventProcessors">Provides <see cref="IEventProcessor">Event Processors</see>.</param>
        /// <param name="tenants">A collection of all <see cref="ITenants">tenants</see>.</param>
        /// <param name="processingHub">An instance of <see cref="IScopedEventProcessingHub" /> for processing <see cref="CommittedEventStream">Committed Event Streams</see>.</param>
        /// <param name="getOffsetRepository">A factory function to return a correctly scoped instance of <see cref="IEventProcessorOffsetRepository" />.</param>
        /// <param name="getUnprocessedEventsFetcher">A factory function to return a correctly scoped instance of <see cref="IFetchUnprocessedEvents" />.</param>
        /// <param name="executionContextManager">The <see cref="ExecutionContextManager" /> for setting the correct execution context for the Event Processors.</param>
        /// <param name="resourceConfiguration"><see cref="IResourceConfiguration"/> for resources.</param>
        /// <param name="scheduler"><see cref="IScheduler"/> to use for scheduling.</param>
        /// <param name="application">Current <see cref="Application"/>.</param>
        /// <param name="boundedContext">Current <see cref="BoundedContext"/>.</param>
        /// <param name="environment">Current <see cref="Execution.Environment">execution environment</see>.</param>
        /// <param name="logger">An instance of <see cref="ILogger" /> for logging.</param>
        public BootProcedure(
            IInstancesOf<IKnowAboutEventProcessors> systemsThatKnowAboutEventProcessors,
            ITenants tenants,
            IScopedEventProcessingHub processingHub,
            FactoryFor<IEventProcessorOffsetRepository> getOffsetRepository,
            FactoryFor<IFetchUnprocessedEvents> getUnprocessedEventsFetcher,
            IExecutionContextManager executionContextManager,
            IResourceConfiguration resourceConfiguration,
            IScheduler scheduler,
            Application application,
            BoundedContext boundedContext,
            Execution.Environment environment,
            ILogger logger)
        {
            _processingHub = processingHub;
            _scheduler = scheduler;
            _logger = logger;
            _tenants = tenants;
            _systemsThatKnowAboutEventProcessors = systemsThatKnowAboutEventProcessors;
            _getOffsetRepository = getOffsetRepository;
            _getUnprocessedEventsFetcher = getUnprocessedEventsFetcher;
            _executionContextManager = executionContextManager;
            _resourceConfiguration = resourceConfiguration;
            _logger = logger;
            _scheduler = scheduler;

            _executionContextManager.SetConstants(application, boundedContext, environment);
        }

        /// <inheritdoc />
        public bool CanPerform()
        {
            var hasAny = _systemsThatKnowAboutEventProcessors.SelectMany(_ => _.ToList()).Any();
            return (hasAny && _resourceConfiguration.IsConfigured) || _canPerformCount-- == 0;
        }

        /// <inheritdoc />
        public void Perform()
        {
            GatherAllEventProcessors();
            _processingHub.BeginProcessingEvents();
        }

        void GatherAllEventProcessors()
        {
            _logger.Information("Gather all event processors");

            _scheduler.PerformForEach(_systemsThatKnowAboutEventProcessors, (system) =>
            {
                _logger.Information($"System that knows about event processors : {system.GetType().AssemblyQualifiedName}");

                _scheduler.PerformForEach(system, (processor) =>
                {
                    _logger.Information($"Processor : {processor.GetType().AssemblyQualifiedName}");

                    _scheduler.PerformForEach(_tenants.All, (t) =>
                    {
                        _logger.Information($"Register scoped event processor for tenant : {t}");
                        _executionContextManager.CurrentFor(t, CorrelationId.New(), Claims.Empty);
                        _processingHub.Register(new ScopedEventProcessor(t, processor, _getOffsetRepository, _getUnprocessedEventsFetcher, _logger));
                    });
                });
            });
        }
    }
}