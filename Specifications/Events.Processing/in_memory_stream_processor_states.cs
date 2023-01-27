// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using System;
using System.Linq;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing;

public class in_memory_stream_processor_states : IStreamProcessorStates
{
    readonly Dictionary<IStreamProcessorId, IStreamProcessorState> states = new();

    public Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken)
    {
        states[streamProcessorId] = streamProcessorState;
        return Task.CompletedTask;
    }

    public Task<Try<IStreamProcessorState>> TryGet(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken)
    {
        if (states.TryGetValue(streamProcessorId, out var state))
        {
            return Task.FromResult(Try<IStreamProcessorState>.Succeeded(state));
        }

        return Task.FromResult(Try<IStreamProcessorState>.Failed(new Exception()));
    }

    public IAsyncEnumerable<StreamProcessorStateWithId> GetAllNonScoped(CancellationToken cancellationToken)
    {
        return states
            .Where(kv => kv.Key.ScopeId.Equals(ScopeId.Default))
            .Select(kv => new StreamProcessorStateWithId(kv.Key, kv.Value))
            .ToAsyncEnumerable();
    }

    public async Task<Try> Persist(IReadOnlyDictionary<IStreamProcessorId, IStreamProcessorState> streamProcessorStates,
        CancellationToken cancellationToken)
    {
        await Task.Yield();

        foreach (var (key, value) in streamProcessorStates)
        {
            states[key] = value;
        }

        return Try.Succeeded();
    }
}