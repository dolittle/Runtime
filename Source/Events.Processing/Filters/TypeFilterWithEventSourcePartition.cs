// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a <see cref="AbstractFilterProcessor"/> that filters by known event types and can partition using an <see cref="EventSourceId"/>.
    /// </summary>
    public class TypeFilterWithEventSourcePartition : AbstractFilterProcessor
    {
        readonly TypeFilterWithEventSourcePartitionDefinition _definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartition"/> class.
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId"/> for the event processor.</param>
        /// <param name="targetStreamId"><see cref="StreamId"/> to write to after filtering.</param>
        /// <param name="definition"><see cref="TypeFilterWithEventSourcePartitionDefinition">Definition</see> for the filter.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public TypeFilterWithEventSourcePartition(
            EventProcessorId eventProcessorId,
            StreamId targetStreamId,
            TypeFilterWithEventSourcePartitionDefinition definition,
            IWriteEventsToStreams eventsToStreamsWriter,
            ILogger logger)
            : base(eventProcessorId, targetStreamId, eventsToStreamsWriter, logger)
        {
            _definition = definition;
        }

        /// <inheritdoc/>
        public override async Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, CancellationToken cancellationToken)
        {
            try
            {
                var included = _definition.Types.Contains(@event.Type.Id);
                var outPartitionId = PartitionId.NotSet;
                if (_definition.Partitioned)
                {
                    outPartitionId = @event.EventSource.Value;
                }

                var filterResult = new SucceededFilteringResult(included, outPartitionId);
                return await Task.FromResult(filterResult).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new FailedFilteringResult($"Failure Message: {ex.Message}\nStack Trace: {ex.StackTrace}")).ConfigureAwait(false);
            }
        }
    }
}