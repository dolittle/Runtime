// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Extensions for <see cref="UncommittedAggregateEvents"/>.
/// </summary>
public static class UncommittedAggregateEventsExtension
{
    /// <summary>
    /// Creates an <see cref="CommitAggregateEventsRequest"/> from the given <see cref="UncommittedAggregateEvents"/> and <see cref="ExecutionContext"/>.
    /// </summary>
    /// <param name="events">The <see cref="UncommittedAggregateEvents"/>.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
    /// <returns>The <see cref="CommitAggregateEventsRequest"/>.</returns>
    public static CommitAggregateEventsRequest ToCommitRequest(this UncommittedAggregateEvents events, ExecutionContext executionContext)
    {
        var request = new CommitAggregateEventsRequest
        {
            CallContext = new CallRequestContext
            {
                ExecutionContext = executionContext.ToProtobuf()
            },
            Events = new Events.Contracts.UncommittedAggregateEvents
            {
                AggregateRootId = events.AggregateRoot.Id.ToProtobuf(),
                EventSourceId = events.EventSource,
                ExpectedAggregateRootVersion = events.ExpectedAggregateRootVersion,
            }
        };
        request.Events.Events.AddRange(events.Select(_ => new Events.Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent
        {
            Content = _.Content,
            Public = _.Public,
            EventType = _.Type.ToProtobuf()
        }));
        return request;
    }
}
