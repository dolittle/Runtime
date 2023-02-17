// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams.Legacy;

[Singleton, PerTenant]
public class EventLogSequenceFromStreamPosition: IGetEventLogSequenceFromStreamPosition
{
    readonly IFetchCommittedEvents _committedEventsFetcher;
    readonly IFilterDefinitions _filterDefinitions;

    public EventLogSequenceFromStreamPosition(IFetchCommittedEvents committedEventsFetcher,
        IFilterDefinitions filterDefinitions)
    {
        _committedEventsFetcher = committedEventsFetcher;
        _filterDefinitions = filterDefinitions;
    }

    public async Task<Try<EventLogSequenceNumber>> TryGetEventLogPositionForStreamProcessor(StreamProcessorId id, StreamPosition streamPosition,
        CancellationToken cancellationToken)
    {
        var tryGetFilter = await _filterDefinitions.TryGetFromStream(id.ScopeId, id.EventProcessorId.Value, cancellationToken);
        if (!tryGetFilter.Success)
        {
            return tryGetFilter.Exception;
        }

        var filter = tryGetFilter.Result;
        if (filter is not TypeFilterWithEventSourcePartitionDefinition typeFilter)
        {
            return Try<EventLogSequenceNumber>.Failed(new ArgumentException("Invalid filter type: " + filter.GetType().Name));
        }

        var artifacts = typeFilter.Types;

        var hopefullyEventLogSequence =
            await _committedEventsFetcher.GetEventLogSequenceFromArtifactSet(id.ScopeId, streamPosition, artifacts, cancellationToken);
        return hopefullyEventLogSequence;
    }
}
