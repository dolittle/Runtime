// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="RemoteFilterDefinition" />.
    /// </summary>
    [SingletonPerTenant]
    public class RemoteFilterValidator : ICanValidateFilterFor<RemoteFilterDefinition>
    {
        readonly IFetchEventsFromStreams _eventsFromStreams;
        readonly IFetchEventTypesFromStreams _eventTypesFromStreams;
        readonly IStreamsMetadata _streamsMetadata;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFilterValidator"/> class.
        /// </summary>
        /// <param name="eventsFromStreams">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="eventTypesFromStreams">The <see cref="IFetchEventTypesFromStreams" />.</param>
        /// <param name="streamsMetadata">The <see cref="IStreamsMetadata" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public RemoteFilterValidator(IFetchEventsFromStreams eventsFromStreams, IFetchEventTypesFromStreams eventTypesFromStreams, IStreamsMetadata streamsMetadata, ILogger logger)
        {
            _eventsFromStreams = eventsFromStreams;
            _eventTypesFromStreams = eventTypesFromStreams;
            _streamsMetadata = streamsMetadata;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Validate(IFilterProcessor<RemoteFilterDefinition> filter, CancellationToken cancellationToken = default)
        {
            var lastProcessedEventLogEvent = await _streamsMetadata.GetLastProcessedEventLogSequenceNumber(filter.Definition.TargetStream, cancellationToken).ConfigureAwait(false);
            if (lastProcessedEventLogEvent == null) return;
            var artifactsFromStream = await _eventTypesFromStreams.FetchTypesInRange(
                filter.Definition.TargetStream,
                new StreamPositionRange(StreamPosition.Start, uint.MaxValue),
                cancellationToken).ConfigureAwait(false);

            var committedEvents = await _eventsFromStreams.FetchRange(filter.Definition.SourceStream, new StreamPositionRange(StreamPosition.Start, lastProcessedEventLogEvent.Value), cancellationToken).ConfigureAwait(false);
            var artifactsFromEventLog = new List<Artifact>();
            foreach (var @event in committedEvents.Select(_ => _.Event))
            {
                var processingResult = await filter.Filter(@event, PartitionId.NotSet, filter.Identifier, cancellationToken).ConfigureAwait(false);
                if (processingResult.IsIncluded) artifactsFromEventLog.Add(@event.Type);
            }

            if (!ArtifactListsAreTheSame(artifactsFromStream, artifactsFromEventLog)) throw new IllegalFilterTransformation(filter.Definition.TargetStream, filter.Definition.SourceStream);
        }

        bool ArtifactListsAreTheSame(IEnumerable<Artifact> oldList, IEnumerable<Artifact> newList) =>
            oldList.LongCount() == newList.LongCount() && oldList.All(newList.Contains);
    }
}