// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using System;

namespace Dolittle.Runtime.Events.Processing;

public class in_memory_stream_processor_states : IStreamProcessorStates
{
    readonly Dictionary<IStreamProcessorId, IStreamProcessorState> states = new();

    public async Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return states.TryGetValue(streamProcessorId, out var state)
            ? Try<IStreamProcessorState>.Succeeded(state)
            : Try<IStreamProcessorState>.Failed(new Exception());
    }

    public async Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken)
    {
        await Task.Yield();
        states[streamProcessorId] = streamProcessorState;
    }
}