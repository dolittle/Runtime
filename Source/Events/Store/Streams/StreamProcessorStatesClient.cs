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
/// Represents an implementation of <see cref="IStreamProcessorStates"/>.
/// </summary>
[PerTenant]
public class StreamProcessorStatesClient : IStreamProcessorStates
{
    readonly StreamProcessorStateClient _streamProcessorStateClient;
    readonly StreamSubscriptionStateClient _streamSubscriptionStateClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProcessorStatesClient"/> class. 
    /// </summary>
    /// <param name="streamProcessorStateClient">The <see cref="Actors.StreamProcessorStateClient"/>.</param>
    /// <param name="streamSubscriptionStateClient">The <see cref="Actors.StreamSubscriptionStateClient"/>.</param>
    public StreamProcessorStatesClient(StreamProcessorStateClient streamProcessorStateClient,
        StreamSubscriptionStateClient streamSubscriptionStateClient)
    {
        _streamProcessorStateClient = streamProcessorStateClient;
        _streamSubscriptionStateClient = streamSubscriptionStateClient;
    }

    /// <inheritdoc />
    public async Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken)
    {
        try
        {
            var processorKey = streamProcessorId.ToProtobuf();
            switch (processorKey.IdCase)
            {
                case StreamProcessorKey.IdOneofCase.StreamProcessorId:
                    var response = await _streamProcessorStateClient.GetByProcessorId(processorKey.StreamProcessorId, cancellationToken).ConfigureAwait(false);
                    return Try<IStreamProcessorState>.Succeeded(response!.Bucket.Single().FromProtobuf());
                case StreamProcessorKey.IdOneofCase.SubscriptionId:
                    var r = await _streamSubscriptionStateClient.GetBySubscriptionId(processorKey.SubscriptionId, cancellationToken).ConfigureAwait(false);
                    return Try<IStreamProcessorState>.Succeeded(r!.Bucket.Single().FromProtobuf());
                case StreamProcessorKey.IdOneofCase.None:
                default:
                    return Try<IStreamProcessorState>.Failed(new ArgumentOutOfRangeException(nameof(streamProcessorId)));
            }
        }
        catch (Exception e)
        {
            return Try<IStreamProcessorState>.Failed(e);
        }
    }

    /// <inheritdoc />
    public async Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken)
    {
        var processorKey = streamProcessorId.ToProtobuf();
        var request = new SetStreamProcessorStateRequest
        {
            StreamKey = processorKey,
            Bucket = streamProcessorState.ToProtobuf()
        };
        switch (processorKey.IdCase)
        {
            case StreamProcessorKey.IdOneofCase.StreamProcessorId:
                await _streamProcessorStateClient.SetByProcessorId(request, cancellationToken).ConfigureAwait(false);
                return;
            case StreamProcessorKey.IdOneofCase.SubscriptionId:
                await _streamSubscriptionStateClient.SetBySubscriptionId(request, cancellationToken).ConfigureAwait(false);
                return;
            case StreamProcessorKey.IdOneofCase.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(streamProcessorId));
        }
    }
}
