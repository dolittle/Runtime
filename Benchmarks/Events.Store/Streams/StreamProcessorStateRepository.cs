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
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.DependencyInjection;
using PartitionedStreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState;

namespace Benchmarks.Events.Store.Streams;

/// <summary>
/// Benchmarks for committing aggregate events to the Event Store.
/// </summary>
public class StreamProcessorStateRepository : JobBase
{
    Dictionary<TenantId, IResilientStreamProcessorStateRepository> _streamProcessorStates;
    (StreamProcessorId, StreamProcessorState)[] _nonPartitionedStates;
    (StreamProcessorId, PartitionedStreamProcessorState)[] _partitionedStates;

    /// <inheritdoc />
    protected override void Setup(IServiceProvider services)
    {
        _streamProcessorStates = ConfiguredTenants.ToDictionary(tenant => tenant, tenant => services.GetRequiredService<Func<TenantId, IResilientStreamProcessorStateRepository>>()(tenant));
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _nonPartitionedStates = Enumerable.Range(0, States).Select(_ =>
        {
            var streamProcessorId = new StreamProcessorId(ScopeId.Default, Guid.NewGuid(), Guid.NewGuid());
            return (streamProcessorId, StreamProcessorState.New);
        }).ToArray();
        _partitionedStates = Enumerable.Range(0, States).Select(_ =>
        {
            var streamProcessorId = new StreamProcessorId(ScopeId.Default, Guid.NewGuid(), Guid.NewGuid());
            return (streamProcessorId, PartitionedStreamProcessorState.New);
        }).ToArray();

        if (PreExistingState)
        {
            Task.WaitAll(SaveAllNonPartitionedStates(), SaveAllPartitionedStates());
        }
    }

    Task SaveAllNonPartitionedStates() => SaveAllStates(_nonPartitionedStates.Select(_ => (_.Item1 as IStreamProcessorId, _.Item2 as IStreamProcessorState)));
    Task SaveAllPartitionedStates() => SaveAllStates(_partitionedStates.Select(_ => (_.Item1 as IStreamProcessorId, _.Item2 as IStreamProcessorState)));
    
    Task GetAllNonPartitionedStates() => GetAllStates(_nonPartitionedStates.Select(_ => (_.Item1 as IStreamProcessorId, _.Item2 as IStreamProcessorState)));
    Task GetAllPartitionedStates() => GetAllStates(_partitionedStates.Select(_ => (_.Item1 as IStreamProcessorId, _.Item2 as IStreamProcessorState)));
    Task SaveAllStates(IEnumerable<(IStreamProcessorId, IStreamProcessorState)> states)
        => Task.WhenAll(ConfiguredTenants.Select(tenant => Task.WhenAll(states.Select(state => _streamProcessorStates[tenant].Persist(state.Item1, state.Item2, CancellationToken.None)))));
    
    Task GetAllStates(IEnumerable<(IStreamProcessorId, IStreamProcessorState)> states)
        => Task.WhenAll(ConfiguredTenants.Select(tenant => Task.WhenAll(states.Select(state => _streamProcessorStates[tenant].TryGetFor(state.Item1, CancellationToken.None)))));

    /// <inheritdoc />
    [Params(1, 2, 5)]
    public override int NumTenants { get; set; }
    public bool PreExistingState { get; set; } = false; 

    [Params(1, 5, 10)]
    public int States { get; set; }
    
    [Params(1, 10, 100)]
    public int PersistsOrGets { get; set; }

    [Benchmark]
    public async Task PersistNonPartitionedState()
    {
        for (var i = 0; i < PersistsOrGets; i++)
        {
            for (var y = 0; y < _nonPartitionedStates.Length; y++)
            {
                var (id, state) = _nonPartitionedStates[y];
                _nonPartitionedStates[y] = (id, state with
                {
                    Position = state.Position + 1,
                    LastSuccessfullyProcessed = DateTimeOffset.UtcNow
                });
            }
            
            await SaveAllNonPartitionedStates();
        }
    }
    [Benchmark]
    public async Task GetNonPartitionedStates()
    {
        for (var i = 0; i < PersistsOrGets; i++)
        {
            await GetAllNonPartitionedStates();
        }
    }
    [Benchmark]
    public async Task PersistPartitionedState()
    {
        for (var i = 0; i < PersistsOrGets; i++)
        {
            for (var y = 0; y < _partitionedStates.Length; y++)
            {
                var (id, state) = _partitionedStates[y];
                _partitionedStates[y] = (id, state with
                {
                    Position = state.Position + 1,
                    LastSuccessfullyProcessed = DateTimeOffset.UtcNow
                });
            }
            
            await SaveAllPartitionedStates();
        }
    }
    [Benchmark]
    public async Task GetPartitionedStates()
    {
        for (var i = 0; i < PersistsOrGets; i++)
        {
            await GetAllPartitionedStates();
        }
    }

    /// <inheritdoc />
    protected override void Cleanup()
    {
    }
}
