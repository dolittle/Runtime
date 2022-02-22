// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public Task<FilterValidationResult> Validate(FilterDefinition persistedDefinition, IFilterProcessor<FilterDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken)
        => PerformValidation(filter, lastUnprocessedEvent, cancellationToken);

    /// <inheritdoc />
    public Task<FilterValidationResult> Validate(PublicFilterDefinition persistedDefinition, IFilterProcessor<PublicFilterDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken)
        => PerformValidation(filter, lastUnprocessedEvent, cancellationToken);

    async Task<FilterValidationResult> PerformValidation(IFilterProcessor<IFilterDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken)
    {
        try
        {
            var streamDefinition = new StreamDefinition(filter.Definition);
            var streamEventsFetcher = await _eventFetchers.GetRangeFetcherFor(filter.Scope, streamDefinition, cancellationToken).ConfigureAwait(false);
            var sourceStreamEventsFetcher = await _eventFetchers.GetRangeFetcherFor(
                filter.Scope,
                new EventLogStreamDefinition(),
                cancellationToken).ConfigureAwait(false);
            var oldStream = await streamEventsFetcher.FetchRange(
                    new StreamPositionRange(StreamPosition.Start, ulong.MaxValue),
                    cancellationToken)
                .ConfigureAwait(false);
            var sourceStreamEvents = await sourceStreamEventsFetcher.FetchRange(
                    new StreamPositionRange(StreamPosition.Start, lastUnprocessedEvent),
                    cancellationToken)
                .ConfigureAwait(false);
            var newStream = new List<StreamEvent>();
            var streamPosition = 0;
            foreach (var @event in sourceStreamEvents.Select(_ => _.Event))
            {
                var executionContext = @event.ExecutionContext; // TODO: What should this be set to?
                var processingResult = await filter.Filter(@event, PartitionId.None, filter.Identifier, executionContext, cancellationToken).ConfigureAwait(false);
                if (processingResult is FailedFiltering failedResult)
                {
                    return FilterValidationResult.Failed(failedResult.FailureReason);
                }
                if (processingResult.IsIncluded)
                {
                    newStream.Add(new StreamEvent(
                        @event,
                        new StreamPosition((ulong)streamPosition++),
                        filter.Definition.TargetStream,
                        processingResult.Partition,
                        streamDefinition.Partitioned));
                }
            }

            var oldStreamList = oldStream.ToList();
            if (newStream.Count != oldStreamList.Count)
            {
                return FilterValidationResult.Failed($"The number of events included in the new stream generated from the filter does not match the old stream.");
            }

            for (var i = 0; i < newStream.Count; i++)
            {
                var newEvent = newStream[i];
                var oldEvent = oldStreamList[i];

                if (newEvent.Event.EventLogSequenceNumber != oldEvent.Event.EventLogSequenceNumber)
                {
                    return FilterValidationResult.Failed($"Event in new stream at position {i} is event {newEvent.Event.EventLogSequenceNumber} while the event in the old stream is event {oldEvent.Event.EventLogSequenceNumber}");
                }

                if (filter.Definition.Partitioned && newEvent.Partition != oldEvent.Partition)
                {
                    return FilterValidationResult.Failed($"Event in new stream at position {i} has is in partition {newEvent.Partition} while the event in the old stream is in partition {oldEvent.Partition}");
                }
            }

            return FilterValidationResult.Succeeded();
        }
        catch (Exception exception)
        {
            return FilterValidationResult.Failed(exception.Message);
        }
    }
}
