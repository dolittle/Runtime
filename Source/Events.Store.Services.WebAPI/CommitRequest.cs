// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

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
    {
        var result = new Contracts.CommitEventsRequest
        {
            CallContext = request.CallContext
        };
        
        result.Events.AddRange(request.Events.Select(_ => new Contracts.UncommittedEvent{EventSourceId = _.EventSource, Public = _.Public, EventType = _.Type, Content = _.Content}));
        return result;
    }
}
