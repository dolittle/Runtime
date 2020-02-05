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
    public class RemoteFilterProcessor : IEventProcessor
    {
        readonly IRemoteFilterService _filter;
        readonly StreamId _targetStreamId;
        readonly IWriteEventsToStreams _eventsToStreamsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFilterProcessor"/> class.
        /// </summary>
        /// <param name="targetStreamId">The stream to to write included events in.</param>
        /// <param name="filter">The <see cref="IRemoteFilterService" />.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="FactoryFor{IWriteEventsToStreams}" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        public RemoteFilterProcessor(
            StreamId targetStreamId,
            IRemoteFilterService filter,
            IWriteEventsToStreams eventsToStreamsWriter,
            ILogger logger)
        {
            Identifier = targetStreamId.Value;
            _targetStreamId = targetStreamId;
            _filter = filter;
            _eventsToStreamsWriter = eventsToStreamsWriter;
            _logger = logger;
            LogMessageBeginning = $"Remote Filter Processor '{Identifier}' with target stream '{_targetStreamId}'";
        }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        string LogMessageBeginning { get; }

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId)
        {
            _logger.Debug($"{LogMessageBeginning} is filtering event '{@event.Metadata.Artifact.Id}' for partition '{partitionId.Value}'");
            var result = await _filter.Filter(@event, partitionId, Identifier).ConfigureAwait(false);
            _logger.Debug($"{LogMessageBeginning} filtered event '{@event.Metadata.Artifact.Id}' for partition '{partitionId.Value}' with result 'Succeeded' = {result.Succeeded}");

            if (result.Succeeded && result.IsIncluded)
            {
                _logger.Debug($"{LogMessageBeginning} writing event '{@event.Metadata.Artifact.Id}' to stream '{_targetStreamId.Value}' in partition '{partitionId.Value}'");
                await _eventsToStreamsWriter.Write(@event, _targetStreamId, result.Partition).ConfigureAwait(false);
            }

            return result;
        }
    }
}