// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

public record CommitEventsRequest(
    CallRequestContext CallContext,
    UncommittedEvent[] Events)
{
    /// <summary>
    /// Converts a <see cref="CommitEventsRequest"/> to a <see cref="Contracts.CommitEventsRequest"/>.
    /// </summary>
    /// <param name="request">The request to convert.</param>
    /// <returns>The converted request.</returns>
    public static implicit operator Contracts.CommitEventsRequest(CommitEventsRequest request)
    {}
}
