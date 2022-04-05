// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents the response of a request to commit events.
/// </summary>
/// <param name="Events">The committed events.</param>
/// <param name="Failure">The failure, if the request failed.</param>
public record CommitResponse(
    CommittedEvent[] Events,
    Failure Failure)
{
    /// <summary>
    /// Converts a <see cref="Contracts.CommitEventsResponse"/> to a <see cref="CommitResponse"/>.
    /// </summary>
    /// <param name="response">The response to convert.</param>
    /// <returns>The converted response.</returns>
    public static implicit operator CommitResponse(Contracts.CommitEventsResponse response)
        => new(response.Events.Select(_ => (CommittedEvent) _).ToArray(), response.Failure);
}
