// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a <see cref="AbstractFilterProcessor{T}"/> that filters by known event types and can partition using an <see cref="EventSourceId"/>.
    /// </summary>
    public class TypeFilterWithEventSourcePartition : AbstractFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartition"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="definition">The<see cref="TypeFilterWithEventSourcePartitionDefinition"/>.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public TypeFilterWithEventSourcePartition(
            ScopeId scope,
            TypeFilterWithEventSourcePartitionDefinition definition,
            IWriteEventsToStreams eventsToStreamsWriter,
            ILogger logger)
            : base(scope, definition, eventsToStreamsWriter, logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId)
        {
            var included = Definition.Types.Contains(@event.Type.Id);
            var outPartitionId = PartitionId.NotSet;
            if (Definition.Partitioned)
            {
                outPartitionId = @event.EventSource.Value;
            }

            return Task.FromResult<IFilterResult>(new SuccessfulFiltering(included, outPartitionId));
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, string failureReason, uint retryCount) =>
            Filter(@event, partitionId, eventProcessorId);
    }
}