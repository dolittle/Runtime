// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing;

public class in_memory_stream_processor_state_repository : IStreamProcessorStateRepository
{
    readonly Dictionary<StreamProcessorId, IStreamProcessorState> states = new();


    public Task<Try<IStreamProcessorState>> TryGet(StreamProcessorId streamProcessorId, CancellationToken cancellationToken)
    {
        if (states.TryGetValue(streamProcessorId, out var state))
        {
            return Task.FromResult(Try<IStreamProcessorState>.Succeeded(state));
        }

        return Task.FromResult(Try<IStreamProcessorState>.Failed(new Exception()));
    }

    public IAsyncEnumerable<StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>> GetForScope(ScopeId scopeId,
        CancellationToken cancellationToken) => throw new System.NotImplementedException();

    public async Task<Try> PersistForScope(ScopeId scope, IReadOnlyDictionary<StreamProcessorId, IStreamProcessorState> streamProcessorStates,
        CancellationToken cancellationToken)
    {
        await Task.Yield();
        
        foreach (var (streamProcessorId, streamProcessorState) in streamProcessorStates)
        {
            states[streamProcessorId] = streamProcessorState;
        }

        return Try.Succeeded;
    }

    public IAsyncEnumerable<StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>> GetNonScoped(CancellationToken cancellationToken) =>
        GetForScope(ScopeId.Default, cancellationToken);
}