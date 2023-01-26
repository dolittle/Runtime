// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
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
public class StreamProcessorStateClient : IStreamProcessorStateRepository
{
    readonly Actors.StreamProcessorStateClient _streamProcessorStateClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorStateClient"/> class. 
    /// </summary>
    /// <param name="streamProcessorStateClient">The <see cref="Actors.StreamProcessorStateClient"/>.</param>
    public StreamProcessorStateClient(Actors.StreamProcessorStateClient streamProcessorStateClient)
    {
        _streamProcessorStateClient = streamProcessorStateClient;
    }

    /// <inheritdoc />
    public async Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _streamProcessorStateClient.Get(streamProcessorId.ToProtobuf(), cancellationToken).ConfigureAwait(false);
            return Try<IStreamProcessorState>.Succeeded(response!.Bucket.Single().FromProtobuf());
        }
        catch (Exception e)
        {
            return Try<IStreamProcessorState>.Failed(e);
        }
    }

    /// <inheritdoc />
    public Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken)
    {
        return _streamProcessorStateClient.SetStreamProcessorPartitionState(new SetStreamProcessorPartitionStateRequest
        {
            StreamKey = streamProcessorId.ToProtobuf(),
            Bucket = streamProcessorState.ToProtobuf()
        }, cancellationToken);
    }
}
