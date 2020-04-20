// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
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

        #nullable enable
        /// <inheritdoc/>
        public override async Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, RetryProcessingState? retryProcessing, CancellationToken cancellationToken)
        {
            try
            {
                var included = Definition.Types.Contains(@event.Type.Id);
                var outPartitionId = PartitionId.NotSet;
                if (Definition.Partitioned)
                {
                    outPartitionId = @event.EventSource.Value;
                }

                var filterResult = new FilteringResult(included, outPartitionId);
                return await Task.FromResult(filterResult).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(
                    new FilteringResult(
                        new ProcessorFailure(ProcessorFailureType.Persistent, $"Failure Message: {ex.Message}\nStack Trace: {ex.StackTrace}"))).ConfigureAwait(false);
            }
        }
    }
}