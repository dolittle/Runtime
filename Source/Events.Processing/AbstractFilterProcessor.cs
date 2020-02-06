// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the filtering of an event.
    /// </summary>
    public abstract class AbstractFilterProcessor : IEventProcessor
    {
        readonly StreamId _targetStreamId;
        readonly IWriteEventsToStreams _eventsToStreamsWriter;
        readonly ILogger _logger;
        readonly string _logMessagePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFilterProcessor"/> class.
        /// </summary>
        /// <param name="targetStreamId">The stream to to write included events in.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="FactoryFor{IWriteEventsToStreams}" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        protected AbstractFilterProcessor(
            StreamId targetStreamId,
            IWriteEventsToStreams eventsToStreamsWriter,
            ILogger logger)
        {
            Identifier = targetStreamId.Value;
            _targetStreamId = targetStreamId;
            _eventsToStreamsWriter = eventsToStreamsWriter;
            _logger = logger;
            _logMessagePrefix = $"Remote Filter Processor '{Identifier}' with target stream '{_targetStreamId}'";
        }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <summary>
        /// Filters the event.
        /// </summary>
        /// <param name="event">The <see cref="Store.CommittedEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of filtering an event.</returns>
        public abstract Task<IFilterResult> Filter(Store.CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId);

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(Store.CommittedEvent @event, PartitionId partitionId)
        {
            _logger.Debug($"{_logMessagePrefix} is filtering event '{@event.Type.Id.Value}' for partition '{partitionId.Value}'");
            var result = await Filter(@event, partitionId, Identifier).ConfigureAwait(false);
            _logger.Debug($"{_logMessagePrefix} filtered event '{@event.Type.Id.Value}' for partition '{partitionId.Value}' with result 'Succeeded' = {result.Succeeded}");

            if (result.Succeeded && result.IsIncluded)
            {
                _logger.Debug($"{_logMessagePrefix} writing event '{@event.Type.Id.Value}' to stream '{_targetStreamId.Value}' in partition '{partitionId.Value}'");
                await _eventsToStreamsWriter.Write(@event, _targetStreamId, result.Partition).ConfigureAwait(false);
            }

            return result;
        }
    }
}