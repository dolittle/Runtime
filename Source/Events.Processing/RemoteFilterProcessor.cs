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
        readonly IWriteEventToStream _eventToStreamWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFilterProcessor"/> class.
        /// </summary>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="targetStreamId">The stream to include events in.</param>
        /// <param name="filter">The <see cref="IRemoteFilterService" />.</param>
        /// <param name="eventToStreamWriter">The <see cref="FactoryFor{IWriteEventToStream}" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        public RemoteFilterProcessor(
            EventProcessorId id,
            StreamId targetStreamId,
            IRemoteFilterService filter,
            IWriteEventToStream eventToStreamWriter,
            ILogger logger)
        {
            Identifier = id;
            _targetStreamId = targetStreamId;
            _filter = filter;
            _eventToStreamWriter = eventToStreamWriter;
            _logger = logger;
            LogMessageBeginning = $"Remote Filter Processor '{Identifier}' with target stream '{_targetStreamId}'";
        }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        string LogMessageBeginning { get; }

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEvent @event)
        {
            _logger.Information($"{LogMessageBeginning} is filtering event '{@event.Metadata.Artifact.Id}'");
            var result = await _filter.Filter(@event, Identifier).ConfigureAwait(false);
            _logger.Information($"{LogMessageBeginning} filtered event '{@event.Metadata.Artifact.Id}' with result Succeeded = {result.Succeeded}");

            // TODO: Handle partition
            if (result.IsIncluded)
            {
                _logger.Information($"{LogMessageBeginning} writing event '{@event.Metadata.Artifact.Id}' to stream '{_targetStreamId.Value}'");
                await _eventToStreamWriter.Write(@event, _targetStreamId).ConfigureAwait(false);
            }

            return result;
        }
    }
}