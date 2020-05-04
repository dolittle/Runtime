// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
    /// Represents an implementation of <see cref="ICanValidateFilterFor{T}" /> that can validate a <see cref="FilterDefinition" />.
    /// </summary>
    [SingletonPerTenant]
    public class RemoteFilterValidator : ICanValidateFilterFor<FilterDefinition>
    {
        readonly IEventFetchers _eventFetchers;
        readonly IFetchEventTypesFromStreams _eventTypesFromStreams;
        readonly IStreamProcessorStateRepository _streamProcessorStateRepository;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFilterValidator"/> class.
        /// </summary>
        /// <param name="eventFetchers">The <see cref="IEventFetchers" />.</param>
        /// <param name="eventTypesFromStreams">The <see cref="IFetchEventTypesFromStreams" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public RemoteFilterValidator(
            IEventFetchers eventFetchers,
            IFetchEventTypesFromStreams eventTypesFromStreams,
            IStreamProcessorStateRepository streamProcessorStates,
            ILogger logger)
        {
            _eventFetchers = eventFetchers;
            _eventTypesFromStreams = eventTypesFromStreams;
            _streamProcessorStateRepository = streamProcessorStates;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FilterValidationResult> Validate(FilterDefinition persistedDefinition, IFilterProcessor<FilterDefinition> filter, CancellationToken cancellationToken)
        {
            if (persistedDefinition == default) return new FilterValidationResult();

            var tryGetState = await _streamProcessorStateRepository.TryGetFor(
                new StreamProcessorId(filter.Scope, filter.Definition.TargetStream.Value, filter.Definition.SourceStream),
                cancellationToken)
                .ConfigureAwait(false);
            if (tryGetState.Success) return new FilterValidationResult();
            var lastUnProcessedEventPosition = tryGetState.Result.Position;
            var artifactsFromTargetStream = await _eventTypesFromStreams.FetchInRange(
                filter.Scope,
                filter.Definition.TargetStream,
                new StreamPositionRange(StreamPosition.Start, uint.MaxValue),
                cancellationToken)
                .ConfigureAwait(false);
            var eventsFetcher = await _eventFetchers.GetFetcherFor(filter.Scope, filter.Definition.TargetStream, cancellationToken).ConfigureAwait(false);
            var sourceStreamEvents = lastUnProcessedEventPosition == 0
                ? Enumerable.Empty<StreamEvent>()
                : await eventsFetcher.FetchRange(
                    new StreamPositionRange(StreamPosition.Start, lastUnProcessedEventPosition),
                    cancellationToken)
                    .ConfigureAwait(false);
            var artifactsFromSourceStream = new List<Artifact>();
            foreach (var @event in sourceStreamEvents.Select(_ => _.Event))
            {
                var processingResult = await filter.Filter(@event, Guid.Empty, filter.Identifier, cancellationToken).ConfigureAwait(false);
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