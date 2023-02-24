// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents an implementation of <see cref="IStreamProcessorStates"/>.
/// </summary>
[Singleton, PerTenant]
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

    /// <exception cref="NullReferenceException"></exception>
    /// <inheritdoc />
    public Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken)
    {
        return Try<IStreamProcessorState>.DoAsync(async () =>
        {
            var processorKey = streamProcessorId.ToProtobuf();
            switch (processorKey.IdCase)
            {
                case StreamProcessorKey.IdOneofCase.StreamProcessorId:
                    var response = await _streamProcessorStateClient.GetByProcessorId(processorKey.StreamProcessorId, cancellationToken).ConfigureAwait(false);
                    return ToResponse(streamProcessorId, response);

                case StreamProcessorKey.IdOneofCase.SubscriptionId:
                    return ToResponse(streamProcessorId,
                        await _streamSubscriptionStateClient.GetBySubscriptionId(processorKey.SubscriptionId, cancellationToken).ConfigureAwait(false));

                case StreamProcessorKey.IdOneofCase.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(streamProcessorId));
            }
        });

        static IStreamProcessorState ToResponse(IStreamProcessorId processorId, StreamProcessorStateResponse? response)
        {
            return response!.Bucket.Count switch
            {
                0 => throw new StreamProcessorStateDoesNotExist(processorId),
                1 => response!.Bucket.Single().FromProtobuf(),
                _ => throw new ArgumentOutOfRangeException("State contains more than one bucket, not yet supported")
            };
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
