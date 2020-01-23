// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessorNew" />that processes the filtering of an event.
    /// </summary>
    public class RemoteFilterProcessor : IEventProcessorNew
    {
        readonly TenantId _tenant;
        readonly IFilterService _filter;
        readonly StreamId _targetStreamId;
        readonly FactoryFor<IWriteEventToStream> _getEventToStreamWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProcessor"/> class.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="targetStreamId">The stream to include events in.</param>
        /// <param name="filter">The <see cref="IFilterService" />.</param>
        /// <param name="getEventToStreamWriter">The <see cref="FactoryFor{IWriteEventToStream}" />.</param>
        public FilterProcessor(
            TenantId tenant,
            EventProcessorId id,
            StreamId targetStreamId,
            IFilterService filter,
            FactoryFor<IWriteEventToStream> getEventToStreamWriter)
        {
            Identifier = id;
            _tenant = tenant;
            _targetStreamId = targetStreamId;
            _filter = filter;
            _getEventToStreamWriter = getEventToStreamWriter;
        }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEventEnvelope @event)
        {
            var result = await _filter.Filter(@event, Identifier).ConfigureAwait(false);
            if (result.IsIncluded)
            {
                var writer = _getEventToStreamWriter();
                await writer.Write(@event, _targetStreamId).ConfigureAwait(false);
            }

            return result;
        }
    }
}