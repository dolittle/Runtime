// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Runtime.Tenancy;
using Dolittle.Scheduling;

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
        readonly IScheduler _scheduler;
        readonly IRemoteFilterService _filterService;
        readonly IStreamProcessorHub _streamProcessorHub;
        readonly FactoryFor<IWriteEventToStream> _getEventToStreamWriter;
        readonly FactoryFor<IFetchNextEvent> _getNextEventFetcher;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterHub"/> class.
        /// </summary>
        /// <param name="tenants">The tenants.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="scheduler">The <see cref="IScheduler" />.</param>
        /// <param name="filterService">The <see cref="IRemoteFilterService" />.</param>
        /// <param name="streamProcessorHub">The <see cref="IStreamProcessorHub" />.</param>
        /// <param name="getEventToStreamWriter">The <see cref="FactoryFor{IWriteEventToStream}" />.</param>
        /// <param name="getNextEventFetcher">The <see cref="FactoryFor{IFetchNextEvent}" />.</param>
        /// <param name="getStreamProcessorStateRepository">The <see cref="FactoryFor{IStreamProcessorStateRepository}" />.</param>
        public FilterHub(
            ITenants tenants,
            IExecutionContextManager executionContextManager,
            IScheduler scheduler,
            IRemoteFilterService filterService,
            IStreamProcessorHub streamProcessorHub,
            FactoryFor<IWriteEventToStream> getEventToStreamWriter,
            FactoryFor<IFetchNextEvent> getNextEventFetcher,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStateRepository)
        {
            _tenants = tenants;
            _executionContextManager = executionContextManager;
            _scheduler = scheduler;
            _filterService = filterService;
            _streamProcessorHub = streamProcessorHub;
            _getEventToStreamWriter = getEventToStreamWriter;
            _getNextEventFetcher = getNextEventFetcher;
            _getStreamProcessorStateRepository = getStreamProcessorStateRepository;
        }

        /// <inheritdoc />
        public void Register(FilterId filterId, StreamId targetStreamId)
        {
            _scheduler.PerformForEach(_tenants.All, tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _streamProcessorHub.Register(
                    new RemoteFilterProcessor(filterId.Value, targetStreamId, _filterService, _getEventToStreamWriter()),
                    StreamId.AllStreamId,
                    _getStreamProcessorStateRepository(),
                    _getNextEventFetcher(),
                    _executionContextManager.Current);
            });
        }
    }
}