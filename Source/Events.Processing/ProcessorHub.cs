// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Tenancy;
using Dolittle.Scheduling;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IProcessorHub" />.
    /// </summary>
    [Singleton]
    public class ProcessorHub : IProcessorHub
    {
        readonly IScheduler _scheduler;
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly IRemoteProcessorService _remoteProcessorService;
        readonly FactoryFor<IStreamProcessorHub> _getStreamProcessorHub;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorHub"/> class.
        /// </summary>
        /// <param name="scheduler">The <see cref="IScheduler" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        /// <param name="remoteProcessorService">The <see cref="IRemoteProcessorService" />.</param>
        /// <param name="getStreamProcessorHub">The <see cref="FactoryFor{IStreamProcessorHub}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ProcessorHub(
            IScheduler scheduler,
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            IRemoteProcessorService remoteProcessorService,
            FactoryFor<IStreamProcessorHub> getStreamProcessorHub,
            ILogger logger)
        {
            _scheduler = scheduler;
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _remoteProcessorService = remoteProcessorService;
            _getStreamProcessorHub = getStreamProcessorHub;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Register(EventProcessorId processorId, StreamId sourceStreamId)
        {
            _logger.Information($"Registering event processor '{processorId.Value}' with source stream '{sourceStreamId.Value}'");
            _tenants.All.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _getStreamProcessorHub().Register(
                    new RemoteEventProcessor(processorId, _remoteProcessorService, _logger),
                    sourceStreamId);
            });
        }
    }
}