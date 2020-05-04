// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        readonly IEventFetchers _eventFetchers;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilterWithEventSourcePartitionValidator"/> class.
        /// </summary>
        /// <param name="eventFetchers">The <see cref="IEventFetchers" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public TypeFilterWithEventSourcePartitionValidator(
            IEventFetchers eventFetchers,
            IStreamProcessorStateRepository streamProcessorStates,
            ILogger logger)
        {
            _eventFetchers = eventFetchers;
            _streamProcessorStates = streamProcessorStates;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<FilterValidationResult> Validate(IFilterDefinition persistedDefinition, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, CancellationToken cancellationToken) =>
            ValidateBasedOffReFilteredStream(persistedDefinition, filter, cancellationToken);

        async Task<FilterValidationResult> ValidateBasedOffReFilteredStream(IFilterDefinition persistedDefinition, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, CancellationToken cancellationToken)
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

            var tryGetState = await _streamProcessorStates.TryGetFor(
                new StreamProcessorId(filter.Scope, filter.Definition.TargetStream.Value, filter.Definition.SourceStream),
                cancellationToken)
                .ConfigureAwait(false);
            if (!tryGetState.Success)
            {
                return new FilterValidationResult();
            }

            var lastUnProcessedEventPosition = tryGetState.Result.Position;
            if (lastUnProcessedEventPosition == 0)
            {
                return new FilterValidationResult();
            }

            var streamEventsFetcher = await _eventFetchers.GetRangeFetcherFor(filter.Scope, filter.Definition.TargetStream, cancellationToken).ConfigureAwait(false);
            var sourceStreamEventsFetcher = await _eventFetchers.GetRangeFetcherFor(filter.Scope, filter.Definition.SourceStream, cancellationToken).ConfigureAwait(false);
            var oldStream = await streamEventsFetcher.FetchRange(
                new StreamPositionRange(StreamPosition.Start, lastUnProcessedEventPosition),
                cancellationToken)
                .ConfigureAwait(false);
            var lastUnprocessedEventLogEventPosition = oldStream.Last().Event.EventLogSequenceNumber;
            var sourceStreamEvents = await sourceStreamEventsFetcher.FetchRange(
                    new StreamPositionRange(StreamPosition.Start, lastUnprocessedEventLogEventPosition),
                    cancellationToken)
                    .ConfigureAwait(false);
            var newStream = new List<StreamEvent>();
            var streamPosition = 0;
            foreach (var @event in sourceStreamEvents.Select(_ => _.Event))
            {
                var processingResult = await filter.Filter(@event, Guid.Empty, filter.Identifier, cancellationToken).ConfigureAwait(false);
                if (processingResult.IsIncluded) newStream.Add(new StreamEvent(@event, new StreamPosition((ulong)streamPosition++), filter.Definition.TargetStream, processingResult.Partition));
            }

            var oldStreamList = oldStream.ToList();
            if (newStream.Count != oldStreamList.Count)
            {
                return new FilterValidationResult($"The number of events included in the new stream generated from the filter does not match the old stream.");
            }

            for (var i = 0; i < newStream.Count; i++)
            {
                var newEvent = newStream[i];
                var oldEvent = oldStreamList[i];

                if (newEvent.Event.EventLogSequenceNumber != oldEvent.Event.EventLogSequenceNumber)
                {
                    return new FilterValidationResult($"Event in new stream at position {i} is event {newEvent.Event.EventLogSequenceNumber} while the event in the old stream is event {oldEvent.Event.EventLogSequenceNumber}");
                }

                if (filter.Definition.Partitioned && newEvent.Partition != oldEvent.Partition)
                {
                    return new FilterValidationResult($"Event in new stream at position {i} has is in partition {newEvent.Partition} while the event in the old stream is in partition {oldEvent.Partition}");
                }
            }

            return new FilterValidationResult();
        }
    }
}