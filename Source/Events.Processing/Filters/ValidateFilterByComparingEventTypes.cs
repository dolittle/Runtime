// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IValidateFilterByComparingEventTypes"/>.
    /// </summary>
    public class ValidateFilterByComparingEventTypes : IValidateFilterByComparingEventTypes
    {
        readonly IEventFetchers _eventFetchers;
        readonly IStreamProcessorStateRepository _streamProcessorStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateFilterByComparingEventTypes"/> class.
        /// </summary>
        /// <param name="eventFetchers">The <see cref="IEventFetchers"/>.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository"/>.</param>
        public ValidateFilterByComparingEventTypes(
            IEventFetchers eventFetchers,
            IStreamProcessorStateRepository streamProcessorStates)
        {
            _eventFetchers = eventFetchers;
            _streamProcessorStates = streamProcessorStates;
        }

        public async Task<FilterValidationResult> Validate(TypeFilterWithEventSourcePartitionDefinition persistedDefinition, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, CancellationToken cancellationToken)
        {
            if (persistedDefinition == default) return new FilterValidationResult();

            if (persistedDefinition.Partitioned != filter.Definition.Partitioned)
            {
                return new FilterValidationResult($"The new stream generated from the filter will not match the old stream. {(persistedDefinition.Partitioned ? "The previous filter is partitioned while the new filter is not" : "The previous filter is not partitioned while the new filter is")}");
            }

            if (persistedDefinition.Public != filter.Definition.Public)
            {
                return new FilterValidationResult($"The new stream generated from the filter will not match the old stream. {(persistedDefinition.Public ? "The previous filter is public while the new filter is not" : "The previous filter is not public while the new filter is")}");
            }

            var changedEventTypes = GetChangedEventTypes(persistedDefinition, filter.Definition);
            if (changedEventTypes.Count == 0)
            {
                return new FilterValidationResult();
            }

            var tryGetState = await _streamProcessorStates.TryGetFor(
                new StreamProcessorId(filter.Scope, filter.Definition.TargetStream.Value, filter.Definition.SourceStream),
                cancellationToken)
                .ConfigureAwait(false);
            if (!tryGetState.Success)
            {
                return new FilterValidationResult();
            }

            var lastUnprocessedEventPosition = tryGetState.Result.Position;
            if (lastUnprocessedEventPosition == 0)
            {
                return new FilterValidationResult();
            }
            
            var streamTypesFetcher = await _eventFetchers.GetTypeFetcherFor(
                filter.Scope,
                new EventLogStreamDefinition(),
                cancellationToken).ConfigureAwait(false);

            var typesInSourceStream = await streamTypesFetcher.FetchInRange(
                new StreamPositionRange(StreamPosition.Start, lastUnprocessedEventPosition),
                cancellationToken).ConfigureAwait(false);

            if (SourceStreamContainsChangedEventTypes(typesInSourceStream, changedEventTypes))
            {
                return new FilterValidationResult("The new filter definition has added or removed event types that have already been filtered");
            }

            return new FilterValidationResult();
        }

        ISet<ArtifactId> GetChangedEventTypes(TypeFilterWithEventSourcePartitionDefinition persistedDefinition, TypeFilterWithEventSourcePartitionDefinition registeredDefinition)
        {
            var addedEventTypes = registeredDefinition.Types.Where(_ => !persistedDefinition.Types.Contains(_));
            var removedEventTypes = persistedDefinition.Types.Where(_ => !registeredDefinition.Types.Contains(_));

            var changedEventTypes = addedEventTypes.Concat(removedEventTypes);
            return new HashSet<ArtifactId>(changedEventTypes);
        }

        bool SourceStreamContainsChangedEventTypes(ISet<Artifact> typesInSourceStream, ISet<ArtifactId> changedEventTypes)
            => typesInSourceStream.Any(_ => changedEventTypes.Contains(_.Id));
    }
}
