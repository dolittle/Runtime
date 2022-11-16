// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents a request to fetch events for an aggregate root instance.
/// </summary>
/// <param name="CallContext">The request call context.</param>
/// <param name="EventSource">The event source of the aggregate root instance.</param>
/// <param name="AggregateRoot">The aggregate root type.</param>
public record FetchForAggregateRequest(
    CallRequestContext CallContext,
    Guid AggregateRoot,
    string EventSource)
{
    /// <summary>
    /// Converts a <see cref="FetchForAggregateRequest"/> to a <see cref="Contracts.FetchForAggregateRequest"/>.
    /// </summary>
    /// <param name="request">The request to convert.</param>
    /// <returns>The converted request.</returns>
    public static implicit operator Contracts.FetchForAggregateInBatchesRequest(FetchForAggregateRequest request)
        => new()
        {
            CallContext = request.CallContext,
            Aggregate = new Aggregate
            {
                AggregateRootId = request.AggregateRoot.ToProtobuf(),
                EventSourceId = request.EventSource
            },
            FetchAllEvents = new FetchAllEventsForAggregateInBatchesRequest(),
        };
}
