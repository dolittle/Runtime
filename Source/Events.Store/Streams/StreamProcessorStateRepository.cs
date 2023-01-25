// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents an implementation of <see cref="IStreamProcessorStateRepository"/>.
/// </summary>
[PerTenant]
public class StreamProcessorStateRepository : IStreamProcessorStateRepository
{
    readonly StreamProcessorStateClient _streamProcessorStateClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorStateRepository"/> class. 
    /// </summary>
    /// <param name="streamProcessorStateClient">The <see cref="StreamProcessorStateClient"/>.</param>
    public StreamProcessorStateRepository(StreamProcessorStateClient streamProcessorStateClient)
    {
        _streamProcessorStateClient = streamProcessorStateClient;
    }

    /// <inheritdoc />
    public async Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken)
    {
        var response = await _streamProcessorStateClient.Get(streamProcessorId.ToProtobuf(), cancellationToken).ConfigureAwait(false);

        return Try<IStreamProcessorState>.Failed(new NotImplementedException());
    }

    /// <inheritdoc />
    public async Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken)
    {
        // var response = await _streamProcessorStateClient.
    }
    
}
