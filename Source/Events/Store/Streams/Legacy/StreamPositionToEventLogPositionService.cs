// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    Task<Try<EventLogSequenceNumber>> TryGetPublicEventLogPosition(StreamPosition streamPosition, CancellationToken cancellationToken);
    Task<Try<EventLogSequenceNumber>> TryGetEventLogPosition(StreamProcessorId id, StreamPosition streamPosition,
        CancellationToken cancellationToken);

    Task<Try<IStreamProcessorState>> WithEventLogSequence(StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState> request,
        CancellationToken cancellationToken);
}

[Singleton, PerTenant]
public class StreamPositionToEventLogPositionService : IMapStreamPositionToEventLogPosition
{
    readonly IEventFetchers _fetchers;
    readonly ILogger<StreamPositionToEventLogPositionService> _logger;

    public StreamPositionToEventLogPositionService(IEventFetchers fetchers, ILogger<StreamPositionToEventLogPositionService> logger)
    {
        _fetchers = fetchers;
        _logger = logger;
    }


    public Task<Try<EventLogSequenceNumber>> TryGetPublicEventLogPosition(StreamPosition streamPosition, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("TODO");
    }

    public async Task<Try<EventLogSequenceNumber>> TryGetEventLogPosition(StreamProcessorId id, StreamPosition streamPosition,
        CancellationToken cancellationToken)
    {
        var fetcher = await _fetchers.GetRangeFetcherFor(id.ScopeId, GetStreamDefinition(id), cancellationToken);
        return await fetcher.FetchEventLogSequenceNumberAsync(id, streamPosition, cancellationToken);
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
        Try<IStreamProcessorState>.DoAsync(async () =>
        {
            var fetcher = await _fetchers.GetRangeFetcherFor(id.ScopeId, GetStreamDefinition(id), cancellationToken);

            {
                return state with
                {
                    Position = state.Position with
                    {
                        EventLogPosition = await fetcher.FetchEventLogSequenceNumberAsync(id, state.Position.StreamPosition, cancellationToken)
                    },
                    FailingPartitions = await Enrich(id, state.FailingPartitions, fetcher, cancellationToken)
                };
            }
        });

    async Task<IDictionary<PartitionId, FailingPartitionState>> Enrich(StreamProcessorId id,
        IDictionary<PartitionId, FailingPartitionState> failingPartitions,
        ICanFetchRangeOfEventsFromStream fetcher, CancellationToken cancellationToken)
    {
        if (failingPartitions.Count == 0) return failingPartitions;

        var enriched = new Dictionary<PartitionId, FailingPartitionState>();

        foreach (var failingPartition in failingPartitions)
        {
            enriched.Add(failingPartition.Key, failingPartition.Value with
            {
                Position = failingPartition.Value.Position with
                {
                    EventLogPosition = await fetcher.FetchEventLogSequenceNumberAsync(id, failingPartition.Value.Position.StreamPosition, cancellationToken)
                },
            });
        }

        return enriched;
    }


    Task<Try<IStreamProcessorState>> FetchForNonPartitioned(StreamProcessorId id, StreamProcessorState state,
        CancellationToken cancellationToken) =>
        Try<IStreamProcessorState>.DoAsync(async () =>
        {
            var fetcher = await _fetchers.GetRangeFetcherFor(id.ScopeId, GetStreamDefinition(id), cancellationToken);

            return state with
            {
                Position = state.Position with
                {
                    EventLogPosition = await fetcher.FetchEventLogSequenceNumberAsync(id, state.Position.StreamPosition, cancellationToken)
                }
            };
        });

    static StreamDefinition GetStreamDefinition(StreamProcessorId streamProcessorId)
    {
        var streamDefinition = new StreamDefinition(new FilterDefinition(streamProcessorId.SourceStreamId, streamProcessorId.SourceStreamId, false));
        return streamDefinition;
    }
}
