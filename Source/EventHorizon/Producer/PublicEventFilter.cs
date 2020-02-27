// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents a <see cref="AbstractFilterProcessor{T}"/> that filters public events.
    /// </summary>
    public class PublicEventFilter : AbstractFilterProcessor<IFilterDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventFilter"/> class.
        /// </summary>
        /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PublicEventFilter(
            IWriteEventsToStreams eventsToStreamsWriter,
            ILogger logger)
            : base(new PublicEventFilterDefinition(), eventsToStreamsWriter, logger)
        {
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, CancellationToken cancellationToken) =>
            Task.FromResult<IFilterResult>(new SucceededFilteringResult(@event.Public, PartitionId.NotSet));
    }
}