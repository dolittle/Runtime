// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;


namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents an implementation of <see cref="ICanValidateFilterFor{TDefinition}"/> for filters defined with <see cref="TypeFilterWithEventSourcePartitionDefinition"/>.
/// </summary>
[Singleton, PerTenant]
public class ValidateFilterByComparingEventTypes : ICanValidateFilterFor<TypeFilterWithEventSourcePartitionDefinition>
{
    readonly IEventFetchers _eventFetchers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateFilterByComparingEventTypes"/> class.
    /// </summary>
    /// <param name="eventFetchers">The <see cref="IEventFetchers"/>.</param>
    public ValidateFilterByComparingEventTypes(
        IEventFetchers eventFetchers)
    {
        _eventFetchers = eventFetchers;
    }

    /// <inheritdoc/>
    public async Task<FilterValidationResult> Validate(TypeFilterWithEventSourcePartitionDefinition persistedDefinition, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filter, StreamPosition lastUnprocessedEvent, CancellationToken cancellationToken)
    {
        try
        {
            var changedEventTypes = GetChangedEventTypes(persistedDefinition, filter.Definition);

            if (EventTypesHaveNotChanged(changedEventTypes))
            {
                return FilterValidationResult.Succeeded();
            }

            var streamTypesFetcher = await _eventFetchers.GetTypeFetcherFor(
                filter.Scope,
                new EventLogStreamDefinition(),
                cancellationToken).ConfigureAwait(false);

            var typesInSourceStream = await streamTypesFetcher.FetchInRange(
                new StreamPositionRange(StreamPosition.Start, lastUnprocessedEvent),
                cancellationToken).ConfigureAwait(false);

            if (SourceStreamContainsChangedEventTypes(typesInSourceStream, changedEventTypes))
            {
                return FilterValidationResult.Failed("The new filter definition has added or removed event types that have already been filtered");
            }

            return FilterValidationResult.Succeeded();
        }
        catch (Exception exception)
        {
            return FilterValidationResult.Failed(exception.Message);
        }
    }

    bool EventTypesHaveNotChanged(ISet<ArtifactId> changedEventTypes) => !changedEventTypes.Any();

    bool SourceStreamContainsChangedEventTypes(ISet<Artifact> typesInSourceStream, ISet<ArtifactId> changedEventTypes)
        => typesInSourceStream.Any(_ => changedEventTypes.Contains(_.Id));

    ISet<ArtifactId> GetChangedEventTypes(TypeFilterWithEventSourcePartitionDefinition persistedDefinition, TypeFilterWithEventSourcePartitionDefinition registeredDefinition)
    {
        var addedEventTypes = registeredDefinition.Types.Where(_ => !persistedDefinition.Types.Contains(_));
        var removedEventTypes = persistedDefinition.Types.Where(_ => !registeredDefinition.Types.Contains(_));

        var changedEventTypes = addedEventTypes.Concat(removedEventTypes);
        return new HashSet<ArtifactId>(changedEventTypes);
    }
}
