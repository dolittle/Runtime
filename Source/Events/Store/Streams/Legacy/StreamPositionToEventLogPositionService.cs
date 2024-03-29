﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState;

namespace Dolittle.Runtime.Events.Store.Streams.Legacy;

public interface IMapStreamPositionToEventLogPosition
{
    Task<Try<IStreamProcessorState>> WithEventLogSequence(StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState> request,
        CancellationToken cancellationToken);
}

[Singleton, PerTenant]
public class StreamPositionToEventLogPositionService : IMapStreamPositionToEventLogPosition
{
    readonly ILogger<StreamPositionToEventLogPositionService> _logger;
    readonly IGetEventLogSequenceFromStreamPosition _getEventLogSequenceFromStreamPosition;

    public StreamPositionToEventLogPositionService(
        ILogger<StreamPositionToEventLogPositionService> logger, IGetEventLogSequenceFromStreamPosition getEventLogSequenceFromStreamPosition)
    {
        _logger = logger;
        _getEventLogSequenceFromStreamPosition = getEventLogSequenceFromStreamPosition;
    }

    public Task<Try<IStreamProcessorState>> WithEventLogSequence(StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState> request,
        CancellationToken cancellationToken)
    {
        if (request.State.Position.EventLogPosition.Value > EventLogSequenceNumber.Initial.Value)
        {
            _logger.EventLogSequenceNumberAlreadySet(request.Id);
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
                EventLogPosition = await GetEventLogPosition(id, true, state.Position.StreamPosition, cancellationToken)
            },
            FailingPartitions = await Enrich(id, state.FailingPartitions, cancellationToken)
        });

    async Task<EventLogSequenceNumber> GetEventLogPosition(StreamProcessorId id, bool partitioned, StreamPosition streamPosition, CancellationToken cancellationToken)
    {
        if(streamPosition == StreamPosition.Start)
        {
            return EventLogSequenceNumber.Initial;
        }
        var eventLogPosition = await _getEventLogSequenceFromStreamPosition.TryGetEventLogPositionForStreamProcessor(id, partitioned, streamPosition, cancellationToken);

        if (!eventLogPosition.Success)
        {
            throw eventLogPosition.Exception;
        }

        return eventLogPosition.Result;
    }

    async Task<ImmutableDictionary<PartitionId, FailingPartitionState>> Enrich(StreamProcessorId id,
        ImmutableDictionary<PartitionId, FailingPartitionState> failingPartitions, CancellationToken cancellationToken)
    {
        if (failingPartitions.Count == 0) return failingPartitions;

        var enriched = new Dictionary<PartitionId, FailingPartitionState>();

        foreach (var failingPartition in failingPartitions)
        {
            enriched.Add(failingPartition.Key, failingPartition.Value with
            {
                Position = failingPartition.Value.Position with
                {
                    EventLogPosition = await GetEventLogPosition(id, true, failingPartition.Value.Position.StreamPosition, cancellationToken)
                },
            });
        }

        return enriched.ToImmutableDictionary();
    }

    Task<Try<IStreamProcessorState>> FetchForNonPartitioned(StreamProcessorId id, StreamProcessorState state,
        CancellationToken cancellationToken) =>
        Try<IStreamProcessorState>.DoAsync(async () => state with
        {
            Position = state.Position with
            {
                EventLogPosition = await GetEventLogPosition(id, false, state.Position.StreamPosition, cancellationToken)
            }
        });
}
