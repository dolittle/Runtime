// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;

namespace Integration.Shared;

public static class EventStoreRequests
{
    public static FetchForAggregateRequest FetchFor(ArtifactId aggregateRootId, EventSourceId eventSourceId, Dolittle.Runtime.Execution.ExecutionContext executionContext)
        => new()
        {
            CallContext = new CallRequestContext
            {
                ExecutionContext = executionContext.ToProtobuf(),
            },
            Aggregate = new Aggregate
            {
                AggregateRootId = aggregateRootId.ToProtobuf(),
                EventSourceId = eventSourceId,
            },
        };
    
    public static FetchForAggregateInBatchesRequest FetchBatchFor(ArtifactId aggregateRootId, EventSourceId eventSourceId, Dolittle.Runtime.Execution.ExecutionContext executionContext)
        => new()
        {
            CallContext = new CallRequestContext
            {
                ExecutionContext = executionContext.ToProtobuf(),
            },
            Aggregate = new Aggregate
            {
                AggregateRootId = aggregateRootId.ToProtobuf(),
                EventSourceId = eventSourceId,
            },
            FetchAllEvents = new FetchAllEventsForAggregateInBatchesRequest(),
        };

    public static FetchForAggregateInBatchesRequest FetchBatchFor(ArtifactId aggregateRootId, EventSourceId eventSourceId, IEnumerable<ArtifactId> eventTypes, Dolittle.Runtime.Execution.ExecutionContext executionContext)
        => new()
        {
            CallContext = new CallRequestContext
            {
                ExecutionContext = executionContext.ToProtobuf(),
            },
            Aggregate = new Aggregate
            {
                AggregateRootId = aggregateRootId.ToProtobuf(),
                EventSourceId = eventSourceId,
            },
            FetchEvents = new FetchEventsForAggregateInBatchesRequest {EventTypes = {eventTypes.Select(_ => new Dolittle.Artifacts.Contracts.Artifact {Id = _.ToProtobuf(), Generation = ArtifactGeneration.First})}},
        };
}
