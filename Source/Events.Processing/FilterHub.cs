// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterHub" />.
    /// </summary>
    [Singleton]
    public class FilterHub : IFilterHub
    {
        readonly StreamProcessorDictionary _streamProcessors;
        readonly IFilterService _filterService;
        readonly IStreamProcessorHub _streamProcessorHub;
        readonly FactoryFor<IWriteEventToStream> _getEventToStreamWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterHub"/> class.
        /// </summary>
        /// <param name="filterService">The <see cref="IFilterService" />.</param>
        /// <param name="streamProcessorHub">The <see cref="IStreamProcessorHub" />.</param>
        /// <param name="getEventToStreamWriter">The <see cref="FactoryFor{IWriteEventToStream}" />.</param>
        public FilterHub(
            IFilterService filterService,
            IStreamProcessorHub streamProcessorHub,
            FactoryFor<IWriteEventToStream> getEventToStreamWriter)
        {
            _filterService = filterService;
            _streamProcessorHub = streamProcessorHub;
            _getEventToStreamWriter = getEventToStreamWriter;
        }

        /// <inheritdoc />
        public void Register(FilterId filterId, StreamId targetStreamId, TenantId tenantId)
        {
            var streamProcessor = _streamProcessorHub.Register(new FilterProcessor(tenantId, filterId.Value, targetStreamId, _filterService, _getEventToStreamWriter), StreamId.AllStreamId, tenantId);
            _streamProcessors.Add(streamProcessor.Key, streamProcessor);
            streamProcessor.Start();
        }
    }
}