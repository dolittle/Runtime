// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Linq;
using System.Threading;
using Dolittle.Artifacts.Contracts;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Machine.Specifications;
using MongoDB.Driver;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence.for_UpdateAggregateVersionsAfterCommit.given;

public class all_dependencies
{
    protected static UpdateAggregateVersionsAfterCommit updater;
    protected static Mock<IAggregateRoots> aggregate_roots;
    protected static ExecutionContext execution_context;
    protected static Mock<IClientSessionHandle> session;
    protected static CancellationToken cancellation_token;
    
    Establish context = () =>
    {
        aggregate_roots = new Mock<IAggregateRoots>();
        execution_context = new ExecutionContext(
            "4dfac023-5fbd-4bc2-a774-72c1743bff0f",
            "efdf7df8-e855-4267-81b3-c5743b0dc3ca",
            Version.NotSet,
            Environment.Development, 
            "61603e6d-4582-4e3b-a5ff-e3a868dbfa6d",
            Claims.Empty, 
            CultureInfo.InvariantCulture);
        session = new Mock<IClientSessionHandle>();
        cancellation_token = default;
        updater = new UpdateAggregateVersionsAfterCommit(aggregate_roots.Object);
    };

    protected static Commit create_commit(EventLogSequenceNumber next_sequence_number, params (ArtifactId aggregate_root_id, EventSourceId event_source_id, AggregateRootVersion expected_aggregate_root_version, int num_events)[] aggregates)
    {
        var commit_builder = new CommitBuilder(next_sequence_number);
        foreach (var (aggregate_root_id, event_source_id, expected_aggregate_root_version, num_events) in aggregates)
        {
            var request = new CommitAggregateEventsRequest
            {
                CallContext = new CallRequestContext
                {
                    ExecutionContext = execution_context.ToProtobuf()
                },
                Events = new Contracts.UncommittedAggregateEvents
                {
                    AggregateRootId = aggregate_root_id.ToProtobuf(),
                    EventSourceId = event_source_id,
                    ExpectedAggregateRootVersion = expected_aggregate_root_version
                }
            };
            request.Events.Events.AddRange(Enumerable.Repeat(new Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent
            {
                Content = "{\"hello\": 42}",
                Public = false,
                EventType = new Runtime.Artifacts.Artifact(aggregate_root_id, ArtifactGeneration.First).ToProtobuf()
            }, num_events));
            var tryAdd = commit_builder.TryAddEventsFrom(request);
            if (!tryAdd.Success)
            {
                throw tryAdd.Exception;
            }
        }

        return commit_builder.Build().Commit;
    }

    protected static void verify_updated_aggregate_root_version_for(
        Times times,
        (EventSourceId event_source_id, ArtifactId aggregate_root) aggregate = default,
        (AggregateRootVersion expected_version, AggregateRootVersion next_version) expected_and_next_version = default)
        => aggregate_roots.Verify(_ => _.IncrementVersionFor(
            Moq.It.IsAny<IClientSessionHandle>(),
            aggregate.event_source_id ?? Moq.It.IsAny<EventSourceId>(),
            aggregate.aggregate_root ?? Moq.It.IsAny<ArtifactId>(),
            expected_and_next_version.expected_version ?? Moq.It.IsAny<AggregateRootVersion>(),
            expected_and_next_version.next_version ?? Moq.It.IsAny<AggregateRootVersion>(),
            cancellation_token), times);

    protected static void verify_no_more_updates() => aggregate_roots.VerifyNoOtherCalls();
}