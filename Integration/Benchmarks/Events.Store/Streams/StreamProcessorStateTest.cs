// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.MongoDB;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState;

namespace Integration.Benchmarks.Events.Store.Streams;

/// <summary>
/// Benchmarks for committing aggregate events to the Event Store.
/// </summary>
public class StreamProcessorStateTest : JobBase
{
    Dictionary<TenantId, (IStreamProcessorStates, IStreamProcessorStateCollections)> _statesCollections;

    (StreamProcessorId, StreamProcessorState)[] _states;

    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _statesCollections = ConfiguredTenants.ToDictionary(tenant => tenant, tenant =>
            (services.GetRequiredService<Func<TenantId, IStreamProcessorStates>>()(tenant), services.GetRequiredService<Func<TenantId, IStreamProcessorStateCollections>>()(tenant)));
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _states = Enumerable.Range(0, States).Select(_ =>
        {
            var streamProcessorId = new StreamProcessorId(ScopeId.Default, Guid.NewGuid(), Guid.NewGuid());
            return (streamProcessorId, StreamProcessorState.New);
        }).ToArray();


        var tasks = ConfiguredTenants
            .SelectMany(tenant => _states
                .Select(state => _statesCollections[tenant].Item1.Persist(state.Item1, state.Item2, CancellationToken.None)));
        Task.WaitAll(tasks.ToArray());
    }
    
    /// <inheritdoc />
    [Params(1, 2)]
    public override int NumTenants { get; set; }

    [Params(1, 5)]
    public int States { get; set; }
    
    [Params(1, 10, 100)]
    public int Persists { get; set; }

    [Benchmark]
    public async Task ReplaceOneAsync()
    {
        for (var i = 0; i < Persists; i++)
        {
            var tasks = ConfiguredTenants.SelectMany(tenant => _states.Select(async idAndState =>
            {
                var (id, state) = idAndState;
                var collection = await _statesCollections[tenant].Item2.Get(id.ScopeId, CancellationToken.None).ConfigureAwait(false);
                await collection.ReplaceOneAsync(CreateFilter(id), new Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.StreamProcessorState(
                    id.EventProcessorId,
                    id.SourceStreamId,
                    state.Position.StreamPosition,
                    state.Position.EventLogPosition,
                    state.RetryTime.UtcDateTime,
                    state.FailureReason,
                    state.ProcessingAttempts,
                    state.LastSuccessfullyProcessed.UtcDateTime,
                    state.IsFailing), new ReplaceOptions{ IsUpsert = true}).ConfigureAwait(false);
            }));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
    [Benchmark]
    public async Task IncrementPosition()
    {
        for (var i = 0; i < Persists; i++)
        {
            var tasks = ConfiguredTenants.SelectMany(tenant => _states.Select(async idAndState =>
            {
                var (id, state) = idAndState;
                var collection = await _statesCollections[tenant].Item2.Get(id.ScopeId, CancellationToken.None).ConfigureAwait(false);
                await collection.UpdateOneAsync(
                    CreateFilter(id),
                    Builders<AbstractStreamProcessorState>.Update.Inc<ulong>(_ => _.Position, 1)).ConfigureAwait(false);
            }));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
    
    [Benchmark]
    public async Task IncrementManyPosition()
    {
        for (var i = 0; i < Persists; i++)
        {
            var tasks = ConfiguredTenants.Select(async tenant =>
            {
                var collection = await _statesCollections[tenant].Item2.Get(ScopeId.Default, CancellationToken.None).ConfigureAwait(false);

                var filters = _states.Select(_ => CreateFilter(_.Item1));

                var filter = filters.First();
                foreach (var filterPart in filters)
                {
                    filter |= filterPart;
                }
                await collection.UpdateManyAsync(filter, Builders<AbstractStreamProcessorState>.Update.Inc<ulong>(_ => _.Position, 1)).ConfigureAwait(false);
            });
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    static FilterDefinition<AbstractStreamProcessorState> CreateFilter(StreamProcessorId id) =>
        Builders<AbstractStreamProcessorState>.Filter.Eq(_ => _.EventProcessor, id.EventProcessorId.Value)
        & Builders<AbstractStreamProcessorState>.Filter.Eq(_ => _.SourceStream, id.SourceStreamId.Value);

    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
