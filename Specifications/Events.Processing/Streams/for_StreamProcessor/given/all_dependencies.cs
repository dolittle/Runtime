// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessor.given;

public class all_dependencies
{
    protected static StreamProcessorId stream_processor_id;
    protected static EventProcessorKind event_processor_kind;
    protected static IPerformActionsForAllTenants on_all_tenants;
    protected static IStreamDefinition stream_definition;
    protected static Mock<Func<TenantId, IEventProcessor>> get_event_processor;
    protected static Mock<Func<TenantId, ICreateScopedStreamProcessors>> get_scoped_stream_processors_creator;
    protected static Mock<ICreateScopedStreamProcessors> scoped_stream_processors_creator;
    protected static Mock<ITenantServiceProviders> tenant_service_providers;
    protected static StreamProcessor stream_processor;
    protected static Mock<ITenants> tenants;
    protected static ExecutionContext execution_context;

    Establish context = () =>
    {
        execution_context = execution_contexts.create();
        tenant_service_providers = new Mock<ITenantServiceProviders>();
        stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), StreamId.New());
        event_processor_kind = "test";
        stream_definition = new StreamDefinition(new FilterDefinition(Guid.NewGuid(), Guid.NewGuid(), false));
        tenants = new Mock<ITenants>();
        scoped_stream_processors_creator = new Mock<ICreateScopedStreamProcessors>();
        get_scoped_stream_processors_creator = new Mock<Func<TenantId, ICreateScopedStreamProcessors>>();
        get_scoped_stream_processors_creator.Setup(_ => _.Invoke(Moq.It.IsAny<TenantId>())).Returns(scoped_stream_processors_creator.Object);
        get_event_processor = new Mock<Func<TenantId, IEventProcessor>>();
        on_all_tenants = new TenantActionPerformer(tenants.Object, tenant_service_providers.Object);
        scoped_stream_processors_creator
            .Setup(_ => _.Create(Moq.It.IsAny<IStreamDefinition>(), Moq.It.IsAny<IStreamProcessorId>(), Moq.It.IsAny<IEventProcessor>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new Mock<AbstractScopedStreamProcessor>(
                new TenantId(Guid.NewGuid()),
                Mock.Of<IStreamProcessorId>(),
                Mock.Of<IStreamDefinition>(),
                Mock.Of<IStreamProcessorState>(),
                Mock.Of<IEventProcessor>(),
                Mock.Of<ICanFetchEventsFromStream>(),
                execution_context,
                Mock.Of<IEventFetcherPolicies>(),
                Mock.Of<IStreamEventWatcher>(),
                Mock.Of<ILogger>()).Object));
        stream_processor = new StreamProcessor(
            stream_processor_id,
            event_processor_kind,
            stream_definition,
            on_all_tenants,
            () => { },
            get_event_processor.Object,
            get_scoped_stream_processors_creator.Object,
            Mock.Of<IMetricsCollector>(),
            Mock.Of<ILogger<StreamProcessor>>(),
            execution_context,
            CancellationToken.None);
    };
}
