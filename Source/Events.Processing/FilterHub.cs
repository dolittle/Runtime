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
    /// Represents an implementation of <see cref="IFilterHub" />.
    /// </summary>
    [Singleton]
    public class FilterHub : IFilterHub
    {
        readonly ITenants _tenants;
        readonly IExecutionContextManager _executionContextManager;
        readonly IRemoteFilterService _filterService;
        readonly FactoryFor<IStreamProcessorHub> _getStreamProcessorHub;
        readonly FactoryFor<IWriteEventToStream> _getEventToStreamWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterHub"/> class.
        /// </summary>
        /// <param name="tenants">The tenants.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="filterService">The <see cref="IRemoteFilterService" />.</param>
        /// <param name="getStreamProcessorHub">The <see cref="FactoryFor{IStreamProcessorHub}" />.</param>
        /// <param name="getEventToStreamWriter">The <see cref="FactoryFor{IWriteEventToStream}" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        public FilterHub(
            ITenants tenants,
            IExecutionContextManager executionContextManager,
            IRemoteFilterService filterService,
            FactoryFor<IStreamProcessorHub> getStreamProcessorHub,
            FactoryFor<IWriteEventToStream> getEventToStreamWriter,
            ILogger logger)
        {
            _tenants = tenants;
            _executionContextManager = executionContextManager;
            _filterService = filterService;
            _getStreamProcessorHub = getStreamProcessorHub;
            _getEventToStreamWriter = getEventToStreamWriter;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Register(FilterId filterId, StreamId targetStreamId)
        {
            _logger.Information($"Registering filter '{filterId.Value}' with target stream '{targetStreamId.Value}' for all tenants.");
            _tenants.All.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _getStreamProcessorHub().Register(
                    new RemoteFilterProcessor(filterId.Value, targetStreamId, _filterService, _getEventToStreamWriter(), _logger),
                    StreamId.AllStreamId);
            });
        }
    }
}