// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    [Singleton]
    public class Filters : IFilters
    {
        readonly ITenants _tenants;
        readonly IExecutionContextManager _executionContextManager;
        readonly IRemoteFilterService _filterService;
        readonly FactoryFor<IStreamProcessorHub> _getStreamProcessorHub;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filters"/> class.
        /// </summary>
        /// <param name="tenants">The tenants.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="filterService">The <see cref="IRemoteFilterService" />.</param>
        /// <param name="getStreamProcessorHub">The <see cref="FactoryFor{IStreamProcessorHub}" />.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{IWriteEventsToStreams}" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        public Filters(
            ITenants tenants,
            IExecutionContextManager executionContextManager,
            IRemoteFilterService filterService,
            FactoryFor<IStreamProcessorHub> getStreamProcessorHub,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            ILogger logger)
        {
            _tenants = tenants;
            _executionContextManager = executionContextManager;
            _filterService = filterService;
            _getStreamProcessorHub = getStreamProcessorHub;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Register(StreamId targetStreamId, StreamId sourceStreamId)
        {
            _logger.Information($"Registering filter with target stream '{targetStreamId.Value}' and source stream '{sourceStreamId.Value}' for all tenants.");
            _tenants.All.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _getStreamProcessorHub().Register(
                    new RemoteFilterProcessor(targetStreamId, _filterService, _getEventsToStreamsWriter(), _logger),
                    sourceStreamId);
            });
        }
    }
}