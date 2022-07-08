// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using CommittedAggregateEvents = Dolittle.Runtime.Events.Store.CommittedAggregateEvents;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using It = Moq.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.given;

public class all_dependencies
{
    protected static EmbeddingId embedding;
    protected static TenantId tenant;
    protected static ExecutionContext execution_context;
    protected static Mock<IUpdateEmbeddingStates> state_updater;
    protected static Mock<IStreamEventWatcher> event_waiter;
    protected static Mock<IEventStore> event_store;
    protected static Mock<IEmbeddingStore> embedding_store;
    protected static Mock<ICalculateStateTransitionEvents> transition_calculator;
    protected static EmbeddingProcessor embedding_processor;
    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        embedding = "f46237c8-e144-4f29-bdcc-610ba075735b";
        tenant = "769272fb-7e6c-4c3e-be40-9cd2fc6c7456";
        state_updater = new Mock<IUpdateEmbeddingStates>();
        event_waiter = new Mock<IStreamEventWatcher>();
        event_store = new Mock<IEventStore>();
        embedding_store = new Mock<IEmbeddingStore>();
        transition_calculator = new Mock<ICalculateStateTransitionEvents>();
        execution_context = execution_contexts.create() with { Tenant = tenant };
        
        embedding_processor = new EmbeddingProcessor(
            embedding,
            tenant,
            execution_context,
            state_updater.Object,
            event_waiter.Object,
            event_store.Object,
            embedding_store.Object,
            transition_calculator.Object,
            Mock.Of<ILogger>());
        cancellation_token = CancellationToken.None;

        state_updater.Setup(_ => _.TryUpdateAll(Moq.It.IsAny<ExecutionContext>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Try.Succeeded()));
        event_waiter.Setup(_ => _.WaitForEvent(ScopeId.Default, StreamId.EventLog, It.IsAny<CancellationToken>())).Returns<ScopeId, StreamId, CancellationToken>((_scope, _stream, cancellationToken) => Task.Delay(Timeout.Infinite, cancellationToken));
    };
    
    protected static CommitAggregateEventsResponse FailedCommitResponse()
        => new()
        {
            Failure = new Exception("something").ToProtobuf()
        };
    protected static CommitAggregateEventsResponse SuccessfulCommitResponse(CommittedAggregateEvents events)
        => new()
        {
            Events = events.ToProtobuf()
        };
}