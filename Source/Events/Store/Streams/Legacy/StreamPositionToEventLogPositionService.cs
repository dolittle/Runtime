// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState;

namespace Dolittle.Runtime.Events.Store.Streams.Legacy;

public interface IMapStreamPositionToEventLogPosition
{
    Task<Try<ProcessingPosition>> TryGetProcessingPosition(EventLogSequenceNumber eventLogPosition, IReadOnlyCollection<ArtifactId> eventTypes,
        CancellationToken cancellationToken);

    Task<Try<EventLogSequenceNumber>> TryGetEventLogPositionForStreamProcessor(StreamProcessorId id, StreamPosition streamPosition,
        CancellationToken cancellationToken);

    Task<Try<IStreamProcessorState>> WithEventLogSequence(StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState> request,
        CancellationToken cancellationToken);
}

[Singleton, PerTenant]
public class StreamPositionToEventLogPositionService : IMapStreamPositionToEventLogPosition
{
    readonly IFetchCommittedEvents _committedEventsFetcher;
    readonly IFilterDefinitions _filterDefinitions;
    readonly ILogger<StreamPositionToEventLogPositionService> _logger;

    public StreamPositionToEventLogPositionService(ILogger<StreamPositionToEventLogPositionService> logger,
        IFetchCommittedEvents committedEventsFetcher, IFilterDefinitions filterDefinitions)
    {
        _logger = logger;
        _committedEventsFetcher = committedEventsFetcher;
        _filterDefinitions = filterDefinitions;
    }

    public Task<Try<ProcessingPosition>> TryGetProcessingPosition(EventLogSequenceNumber eventLogPosition, IReadOnlyCollection<ArtifactId> eventTypes,
        CancellationToken cancellationToken)
    {
        return Try<ProcessingPosition>.DoAsync(async () =>
        {
            var streamPosition =
                await _committedEventsFetcher.GetStreamPositionFromArtifactSet(ScopeId.Default, eventLogPosition, eventTypes, cancellationToken);

            return new ProcessingPosition(streamPosition, eventLogPosition);
        });
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


    public Task<Try<IStreamProcessorState>> WithEventLogSequence(StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState> request,
        CancellationToken cancellationToken)
    {
        if (request.State.Position.EventLogPosition.Value > EventLogSequenceNumber.Initial.Value)
        {
            _logger.LogInformation("EventLogSequenceNumber is already set - skipping");
            return Task.FromResult(Try<IStreamProcessorState>.Succeeded(request.State));
        }

        return request.State switch
        {
            Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState state => FetchForPartitioned(request.Id, state, cancellationToken),
            StreamProcessorState state => FetchForNonPartitioned(request.Id, state, cancellationToken),
            _ => Task.FromResult(Try<IStreamProcessorState>.Failed(new ArgumentOutOfRangeException(nameof(request))))
        };
    }

    Task<Try<IStreamProcessorState>> FetchForPartitioned(
        StreamProcessorId id,
        Processing.Streams.Partitioned.StreamProcessorState state,
        CancellationToken cancellationToken) =>
        Try<IStreamProcessorState>.DoAsync(async () => state with
        {
            Position = state.Position with
            {
                EventLogPosition = await GetEventLogPosition(id, state.Position.StreamPosition, cancellationToken)
            },
            FailingPartitions = await Enrich(id, state.FailingPartitions, cancellationToken)
        });

    async Task<EventLogSequenceNumber> GetEventLogPosition(StreamProcessorId id, StreamPosition streamPosition, CancellationToken cancellationToken)
    {
        var eventLogPosition = await TryGetEventLogPositionForStreamProcessor(id, streamPosition, cancellationToken);

        if (!eventLogPosition.Success)
        {
            throw eventLogPosition.Exception;
        }

        return eventLogPosition.Result;
    }

    async Task<IDictionary<PartitionId, FailingPartitionState>> Enrich(StreamProcessorId id,
        IDictionary<PartitionId, FailingPartitionState> failingPartitions, CancellationToken cancellationToken)
    {
        if (failingPartitions.Count == 0) return failingPartitions;

        var enriched = new Dictionary<PartitionId, FailingPartitionState>();

        foreach (var failingPartition in failingPartitions)
        {
            enriched.Add(failingPartition.Key, failingPartition.Value with
            {
                Position = failingPartition.Value.Position with
                {
                    EventLogPosition = await GetEventLogPosition(id, failingPartition.Value.Position.StreamPosition, cancellationToken)
                },
            });
        }

        return enriched;
    }


    Task<Try<IStreamProcessorState>> FetchForNonPartitioned(StreamProcessorId id, StreamProcessorState state,
        CancellationToken cancellationToken) =>
        Try<IStreamProcessorState>.DoAsync(async () => state with
        {
            Position = state.Position with
            {
                EventLogPosition = await GetEventLogPosition(id, state.Position.StreamPosition, cancellationToken)
            }
        });

    // static StreamDefinition GetStreamDefinition(StreamProcessorId streamProcessorId)
    // {
    //     var streamDefinition = new StreamDefinition(new FilterDefinition(streamProcessorId.SourceStreamId, streamProcessorId.SourceStreamId, false));
    //     return streamDefinition;
    // }
}
