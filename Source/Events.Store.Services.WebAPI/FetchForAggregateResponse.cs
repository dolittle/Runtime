// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents the response of a request to fetch committed aggregate events.
/// </summary>
/// <param name="Events">The committed aggregate events.</param>
/// <param name="Failure">The failure, if the request failed.</param>
public record FetchForAggregateResponse(
    CommittedAggregateEvents Events,
    Failure Failure)
{
    /// <summary>
    /// Converts a <see cref="Contracts.FetchForAggregateResponse"/> to a <see cref="Contracts.FetchForAggregateResponse"/>.
    /// </summary>
    /// <param name="response">The response to convert.</param>
    /// <returns>The converted response.</returns>
    public static implicit operator FetchForAggregateResponse(Contracts.FetchForAggregateResponse response)
        => new(response.Events, response.Failure);
}
