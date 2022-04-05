// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents the response of a request to commit aggregate events.
/// </summary>
/// <param name="Events">The committed aggregate events.</param>
/// <param name="Failure">The failure, if the request failed.</param>
public record CommitForAggregateResponse(
    CommittedAggregateEvents Events,
    Failure Failure)
{
    /// <summary>
    /// Converts a <see cref="Contracts.CommitAggregateEventsResponse"/> to a <see cref="CommitForAggregateResponse"/>.
    /// </summary>
    /// <param name="response">The response to convert.</param>
    /// <returns>The converted response.</returns>
    public static implicit operator CommitForAggregateResponse(Contracts.CommitAggregateEventsResponse response)
        => new(response.Events, response.Failure);
}
