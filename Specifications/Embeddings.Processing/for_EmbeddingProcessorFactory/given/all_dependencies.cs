// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using It = Moq.It;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessorFactory.given;

public class all_dependencies
{
    protected static ExecutionContext execution_context;
    protected static Mock<IEventStore> event_store;
    protected static Mock<IFetchCommittedEvents> committed_events_fetcher;
    protected static Mock<IEmbeddingStore> embedding_store;
    protected static Mock<IStreamEventWatcher> stream_event_watcher;
    protected static Mock<IDetectEmbeddingLoops> detect_embedding_loops;
    protected static Mock<ICompareStates> compare_states;
    protected static EmbeddingProcessorFactory factory;
    protected static TenantId tenant_id;

    Establish context = () =>
    {
        tenant_id = "edacfd6f-ab94-49bd-bb0d-15b0041a870b";
        execution_context = execution_contexts.create();
        event_store = new Mock<IEventStore>();
        committed_events_fetcher = new Mock<IFetchCommittedEvents>();
        embedding_store = new Mock<IEmbeddingStore>();
        stream_event_watcher = new Mock<IStreamEventWatcher>();
        detect_embedding_loops = new Mock<IDetectEmbeddingLoops>();
        compare_states = new Mock<ICompareStates>();
        factory = new EmbeddingProcessorFactory(
            tenant_id,
            event_store.Object,
            committed_events_fetcher.Object,
            embedding_store.Object,
            stream_event_watcher.Object,
            compare_states.Object,
            detect_embedding_loops.Object,
            NullLoggerFactory.Instance);
    };
}