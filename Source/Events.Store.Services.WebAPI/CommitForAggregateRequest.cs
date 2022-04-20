// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Linq;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

public record CommitForAggregateRequest(
    CallRequestContext CallContext,
    UncommittedAggregateEvents AggregateEvents)
{
    /// <summary>
    /// Converts a <see cref="CommitEventsRequest"/> to a <see cref="Contracts.CommitEventsRequest"/>.
    /// </summary>
    /// <param name="request">The request to convert.</param>
    /// <returns>The converted request.</returns>
    public static implicit operator Contracts.CommitAggregateEventsRequest(CommitForAggregateRequest request)
    {
        var result = new Contracts.CommitAggregateEventsRequest()
        {
            CallContext = request.CallContext,
        };
        result.Events = new Contracts.UncommittedAggregateEvents
        {
            AggregateRootId = request.AggregateEvents.AggregateRoot.ToProtobuf(),
            EventSourceId = request.AggregateEvents.EventSource,
            ExpectedAggregateRootVersion = request.AggregateEvents.AggregateRootVersion
        };
        result.Events.Events.AddRange(request.AggregateEvents.Events.Select(_ => new Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent{Content = _.Content, Public = _.Public, EventType = _.Type}));
        return result;
    }
}
