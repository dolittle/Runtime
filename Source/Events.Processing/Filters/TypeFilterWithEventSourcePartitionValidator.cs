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
    /// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    [SingletonPerTenant]
    public class TypeFilterWithEventSourcePartitionValidator : ICanValidateFilterFor<TypeFilterWithEventSourcePartitionDefinition>
    {
        readonly IFilterDefinitionRepositoryFor<TypeFilterWithEventSourcePartitionDefinition> _filterDefinitionRepository;
        readonly IFetchEventsFromStreams _eventsFromStreams;
        readonly IFetchEventTypesFromStreams _eventTypesFromStreams;
        readonly IStreamsMetadata _streamsMetadata;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartitionValidator"/> class.
        /// </summary>
        /// <param name="filterDefinitionRepository">The <see cref="IFilterDefinitionRepositoryFor{TDefinition}" /> for <see cref="TypeFilterWithEventSourcePartitionDefinition" />.</param>
        /// <param name="eventsFromStreams">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="eventTypesFromStreams">The <see cref="IFetchEventTypesFromStreams" />.</param>
        /// <param name="streamsMetadata">The <see cref="IStreamsMetadata" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public TypeFilterWithEventSourcePartitionValidator(
            IFilterDefinitionRepositoryFor<TypeFilterWithEventSourcePartitionDefinition> filterDefinitionRepository,
            IFetchEventsFromStreams eventsFromStreams,
            IFetchEventTypesFromStreams eventTypesFromStreams,
            IStreamsMetadata streamsMetadata,
            ILogger logger)
        {
            _filterDefinitionRepository = filterDefinitionRepository;
            _eventsFromStreams = eventsFromStreams;
            _eventTypesFromStreams = eventTypesFromStreams;
            _streamsMetadata = streamsMetadata;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task Validate(IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, CancellationToken cancellationToken = default) =>
            ValidateBasedOffReFilteredStream(filter, cancellationToken);

        async Task ValidateBasedOffReFilteredStream(IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, CancellationToken cancellationToken)
        {
            var lastProcessedEventLogEvent = await _streamsMetadata.GetLastProcessedEventLogSequenceNumber(filter.Definition.TargetStream, cancellationToken).ConfigureAwait(false);
            if (lastProcessedEventLogEvent == null) return;
            var artifactsFromStream = await _eventTypesFromStreams.FetchTypesInRange(filter.Definition.TargetStream, new StreamPositionRange(StreamPosition.Start, uint.MaxValue), cancellationToken).ConfigureAwait(false);

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