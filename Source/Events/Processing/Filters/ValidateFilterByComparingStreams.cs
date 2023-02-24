// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;


namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents an implementation of <see cref="ICanValidateFilterFor{TDefinition}"/> for filters defined with <see cref="FilterDefinition"/> or <see cref="PublicFilterDefinition"/>.
/// </summary>
[Singleton, PerTenant]
public class ValidateFilterByComparingStreams : ICanValidateFilterFor<FilterDefinition>, ICanValidateFilterFor<PublicFilterDefinition>
{
    readonly IEventFetchers _eventFetchers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateFilterByComparingStreams"/> class.
    /// </summary>
    /// <param name="eventFetchers">The <see cref="IEventFetchers" />.</param>
    public ValidateFilterByComparingStreams(IEventFetchers eventFetchers)
    {
        _eventFetchers = eventFetchers;
    }

    /// <inheritdoc />
    public Task<FilterValidationResult> Validate(FilterDefinition persistedDefinition, IFilterProcessor<FilterDefinition> filter, ProcessingPosition lastUnprocessedEvent, CancellationToken cancellationToken)
        => PerformValidation(filter, lastUnprocessedEvent, cancellationToken);

    /// <inheritdoc />
    public Task<FilterValidationResult> Validate(PublicFilterDefinition persistedDefinition, IFilterProcessor<PublicFilterDefinition> filter, ProcessingPosition lastUnprocessedEvent, CancellationToken cancellationToken)
        => PerformValidation(filter, lastUnprocessedEvent, cancellationToken);

    async Task<FilterValidationResult> PerformValidation(IFilterProcessor<IFilterDefinition> filter, ProcessingPosition lastUnprocessedEvent, CancellationToken cancellationToken)
    {
        try
        {
            var streamDefinition = new StreamDefinition(filter.Definition);
            var oldStreamEventsFetcher = await _eventFetchers.GetRangeFetcherFor(
                filter.Scope,
                streamDefinition,
                cancellationToken).ConfigureAwait(false);
            var eventLogFetcher = await _eventFetchers.GetRangeFetcherFor(
                filter.Scope,
                new EventLogStreamDefinition(),
                cancellationToken).ConfigureAwait(false);

            var oldStream = oldStreamEventsFetcher.FetchRange(
                    new StreamPositionRange(StreamPosition.Start, ulong.MaxValue),
                    cancellationToken);
            var eventLogStream = eventLogFetcher.FetchRange(new StreamPositionRange(StreamPosition.Start, lastUnprocessedEvent.StreamPosition), cancellationToken);
            await using var oldStreamEnumerator = oldStream.GetAsyncEnumerator(cancellationToken);
            var newStreamPosition = 0;
            await foreach (var eventFromEventLog in eventLogStream.WithCancellation(cancellationToken))
            {
                var filteringResult = await filter.Filter(eventFromEventLog.Event, PartitionId.None, filter.Identifier, eventFromEventLog.Event.ExecutionContext, cancellationToken).ConfigureAwait(false);
                if (filteringResult is FailedFiltering failedResult)
                {
                    return FilterValidationResult.Failed(failedResult.FailureReason);
                }

                if (!filteringResult.IsIncluded)
                {
                    continue;
                }

                if (!await oldStreamEnumerator.MoveNextAsync())
                {
                    return FilterValidationResult.Failed("The number of events included in the new stream generated from the filter does not match the old stream.");
                }
                var oldStreamEvent = oldStreamEnumerator.Current;
                var filteredEvent = new StreamEvent(
                        eventFromEventLog.Event,
                        new StreamPosition((ulong) newStreamPosition++),
                        filter.Definition.TargetStream,
                        filteringResult.Partition,
                        streamDefinition.Partitioned);
                if (EventsAreNotEqual(filter.Definition, filteredEvent, oldStreamEvent, out var failedValidation))
                {
                    return failedValidation;
                }
            }
            
            if (await oldStreamEnumerator.MoveNextAsync())
            {
                return FilterValidationResult.Failed($"The number of events included in the new stream generated from the filter does not match the old stream.");
            }

            return FilterValidationResult.Succeeded();
        }
        catch (Exception exception)
        {
            return FilterValidationResult.Failed(exception.Message);
        }
    }

    static bool EventsAreNotEqual(IFilterDefinition filterDefinition, StreamEvent newEvent, StreamEvent oldEvent, [NotNullWhen(true)] out FilterValidationResult? failedResult)
    {
        failedResult = default;
        if (newEvent.Event.EventLogSequenceNumber != oldEvent.Event.EventLogSequenceNumber)
        {
            failedResult = FilterValidationResult.Failed($"Event in new stream at position {newEvent.Position} is event {newEvent.Event.EventLogSequenceNumber} while the event in the old stream is event {oldEvent.Event.EventLogSequenceNumber}");
            return true;
        }

        if (filterDefinition.Partitioned && newEvent.Partition != oldEvent.Partition)
        {
            failedResult = FilterValidationResult.Failed($"Event in new stream at position {newEvent.Position} has is in partition {newEvent.Partition} while the event in the old stream is in partition {oldEvent.Partition}");
            return true;
        }
        return false;
    }
}
