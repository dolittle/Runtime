// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    [SingletonPerTenant]
    public class TypeFilterWithEventSourcePartitionValidator : ICanValidateFilterFor<TypeFilterWithEventSourcePartitionDefinition>
    {
        readonly IFetchEventsFromStreams _eventsFromStreams;
        readonly IFetchEventTypesFromStreams _eventTypesFromStreams;
        readonly IStreamProcessorStateRepository _streamProcessorStateRepository;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartitionValidator"/> class.
        /// </summary>
        /// <param name="eventsFromStreams">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="eventTypesFromStreams">The <see cref="IFetchEventTypesFromStreams" />.</param>
        /// <param name="streamProcessorStateRepository">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public TypeFilterWithEventSourcePartitionValidator(
            IFetchEventsFromStreams eventsFromStreams,
            IFetchEventTypesFromStreams eventTypesFromStreams,
            IStreamProcessorStateRepository streamProcessorStateRepository,
            ILogger logger)
        {
            _eventsFromStreams = eventsFromStreams;
            _eventTypesFromStreams = eventTypesFromStreams;
            _streamProcessorStateRepository = streamProcessorStateRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<FilterValidationResult> Validate(IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, CancellationToken cancellationToken) =>
            ValidateBasedOffReFilteredStream(filter, cancellationToken);

        async Task<FilterValidationResult> ValidateBasedOffReFilteredStream(IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, CancellationToken cancellationToken)
        {
            var streamProcessorState = await _streamProcessorStateRepository
                .GetOrAddNew(
                    new StreamProcessorId(filter.Scope, filter.Definition.TargetStream.Value, filter.Definition.SourceStream),
                    cancellationToken)
                .ConfigureAwait(false);
            var lastUnProcessedEventPosition = streamProcessorState.Position;
            var artifactsFromTargetStream = await _eventTypesFromStreams.FetchInRange(
                    filter.Scope,
                    filter.Definition.TargetStream,
                    new StreamPositionRange(StreamPosition.Start, uint.MaxValue),
                    cancellationToken)
                .ConfigureAwait(false);
            var sourceStreamEvents = lastUnProcessedEventPosition == 0
                ? Enumerable.Empty<StreamEvent>()
                : await _eventsFromStreams.FetchRange(
                    filter.Scope,
                    filter.Definition.SourceStream,
                    new StreamPositionRange(StreamPosition.Start, lastUnProcessedEventPosition),
                    cancellationToken)
                    .ConfigureAwait(false);
            var artifactsFromSourceStream = new List<Artifact>();
            foreach (var @event in sourceStreamEvents.Select(_ => _.Event))
            {
                var processingResult = await filter.Filter(@event, PartitionId.NotSet, filter.Identifier, cancellationToken).ConfigureAwait(false);
                if (processingResult.IsIncluded) artifactsFromSourceStream.Add(@event.Type);
            }

            if (!ArtifactListsAreTheSame(artifactsFromTargetStream, artifactsFromSourceStream))
            {
                return new FilterValidationResult($"The new stream generated from the filter does not match the old stream.");
            }

            return new FilterValidationResult();
        }

        bool ArtifactListsAreTheSame(IEnumerable<Artifact> oldList, IEnumerable<Artifact> newList) =>
            oldList.LongCount() == newList.LongCount() && oldList.All(newList.Contains);
    }
}