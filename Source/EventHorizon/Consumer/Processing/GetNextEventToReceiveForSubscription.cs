// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Represents an implementation of <see cref="IGetNextEventToReceiveForSubscription"/>.
/// </summary>
public class GetNextEventToReceiveForSubscription : IGetNextEventToReceiveForSubscription
{
    readonly IStreamProcessorStates _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetNextEventToReceiveForSubscription"/> class.
    /// </summary>
    /// <param name="repository">The stream processor state repository to use for getting subscription states.</param>
    public GetNextEventToReceiveForSubscription(IStreamProcessorStates repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task<StreamPosition> GetNextEventToReceiveFor(SubscriptionId subscriptionId, CancellationToken cancellationToken)
    {
        var tryGetState = await _repository.TryGetFor(subscriptionId, cancellationToken).ConfigureAwait(false);
        return tryGetState.Success
            ? tryGetState.Result.Position
            : StreamPosition.Start;
    }
}