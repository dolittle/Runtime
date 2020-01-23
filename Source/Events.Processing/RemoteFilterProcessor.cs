// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the filtering of an event.
    /// </summary>
    public class RemoteFilterProcessor : IEventProcessor
    {
        readonly TenantId _tenant;
        readonly IRemoteFilterService _filter;
        readonly StreamId _targetStreamId;
        readonly IWriteEventToStream _eventToStreamWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFilterProcessor"/> class.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" />.</param>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="targetStreamId">The stream to include events in.</param>
        /// <param name="filter">The <see cref="IRemoteFilterService" />.</param>
        /// <param name="eventToStreamWriter">The <see cref="FactoryFor{IWriteEventToStream}" />.</param>
        public RemoteFilterProcessor(
            TenantId tenant,
            EventProcessorId id,
            StreamId targetStreamId,
            IRemoteFilterService filter,
            IWriteEventToStream eventToStreamWriter)
        {
            Identifier = id;
            _tenant = tenant;
            _targetStreamId = targetStreamId;
            _filter = filter;
            _eventToStreamWriter = eventToStreamWriter;
        }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEventEnvelope @event)
        {
            var result = await _filter.Filter(@event, Identifier).ConfigureAwait(false);
            if (result.IsIncluded) await _eventToStreamWriter.Write(@event, _targetStreamId).ConfigureAwait(false);

            return result;
        }
    }
}