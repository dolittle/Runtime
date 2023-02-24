// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using ReverseCallDispatcherType = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;
using Version = Dolittle.Runtime.Domain.Platform.Version;
using static Moq.It;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.EventHandlers.Actors;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Legacy;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Proto;
using Failure = Dolittle.Runtime.Protobuf.Failure;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler.given;

public class all_dependencies
{
    protected static Mock<IStreamProcessors> stream_processors;
    protected static Mock<IValidateFilterForAllTenants> filter_validation;
    protected static Mock<IStreamDefinitions> stream_definitions;
    protected static Mock<ReverseCallDispatcherType> reverse_call_dispatcher;
    protected static Mock<IWriteEventsToStreams> stream_writer;
    protected static EventHandlerRegistrationArguments arguments;
    protected static Mock<IMetricsCollector> metrics_collector;
    protected static ILoggerFactory logger_factory;
    protected static Func<TenantId, IWriteEventsToStreams> factory_for_stream_writer;
    protected static CancellationToken cancellation_token;
    protected static ExecutionContext execution_context;
    protected static EventProcessorId event_handler_id = Guid.Parse("6afafdf6-33e8-4135-b271-2f7634b70a7f");
    protected static ScopeId scope = Guid.Empty;
    protected static MicroserviceId MicroserviceId = Guid.Parse("cfa85cf2-b463-4e05-8877-211fd3d1237a");
    protected static TenantId tenant = TenantId.Development;
    protected static Version version = Version.NotSet;
    protected static Environment environment = Environment.Production;
    protected static CorrelationId correlation_id = Guid.Parse("720040db-3a60-4755-8d77-31ba4895be46");
    protected static ActivitySpanId span_id = ActivitySpanId.CreateRandom();
    protected static Claims claims = Claims.Empty;
    protected static CultureInfo culture_info = CultureInfo.InvariantCulture;
    protected static ActorSystem actor_system;
    protected static CreateStreamProcessorActorProps create_processor_props;
    protected static CreateTenantScopedStreamProcessorProps create_scoped_processor_props;
    protected static in_memory_stream_processor_states stream_processor_states;

    protected static ITenants tenants =
        new Tenants(Options.Create(new TenantsConfiguration(new Dictionary<TenantId, TenantConfiguration>
        {
            { tenant, new TenantConfiguration() }
        })));

    protected static Failure failure;

    private Establish context = () =>
    {
        stream_processors = new Mock<IStreamProcessors>(MockBehavior.Strict);
        filter_validation = new Mock<IValidateFilterForAllTenants>(MockBehavior.Strict);
        stream_definitions = new Mock<IStreamDefinitions>(MockBehavior.Strict);
        reverse_call_dispatcher = new Mock<ReverseCallDispatcherType>();
        reverse_call_dispatcher.Setup(
            _ => _.Reject(IsAny<EventHandlerRegistrationResponse>(), IsAny<CancellationToken>())
        ).Callback((EventHandlerRegistrationResponse e, CancellationToken ct) => failure = e.Failure);

        stream_writer = new Mock<IWriteEventsToStreams>(MockBehavior.Strict);
        metrics_collector = new Mock<IMetricsCollector>();
        logger_factory = new NullLoggerFactory();
        execution_context = new ExecutionContext(
            MicroserviceId,
            tenant,
            version,
            environment,
            correlation_id,
            span_id,
            claims,
            culture_info);

        arguments = new EventHandlerRegistrationArguments(
            execution_context,
            event_handler_id,
            Array.Empty<ArtifactId>(),
            false,
            scope,
            "alias");

        factory_for_stream_writer = (tenant_id) => stream_writer.Object;

        actor_system = new ActorSystem();

        stream_processor_states = new in_memory_stream_processor_states();

        var eventSubscriber = new Mock<IStreamEventSubscriber>();
        var eventLogPositionEnricher = new Mock<IMapStreamPositionToEventLogPosition>().Object;
        
        create_scoped_processor_props = (StreamProcessorId streamProcessorId,
            TypeFilterWithEventSourcePartitionDefinition filterDefinition,
            IEventProcessor processor,
            ExecutionContext executionContext,
            ScopedStreamProcessorProcessedEvent onProcessed,
            ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
            TenantId tenantId) => Props.FromProducer(() => new TenantScopedStreamProcessorActor(
            streamProcessorId,
        filterDefinition,
            processor,
            eventSubscriber.Object,
            stream_processor_states,
        executionContext,
            eventLogPositionEnricher,
        NullLogger<TenantScopedStreamProcessorActor>.Instance, 
            onProcessed,
        onFailedToProcess,
            tenantId
            ));


        create_processor_props = (
            StreamProcessorId streamProcessorId,
            IStreamDefinition streamDefinition,
            Func<TenantId, IEventProcessor> createEventProcessorFor,
            StreamProcessorProcessedEvent processedEvent,
            StreamProcessorFailedToProcessEvent failedToProcessEvent,
            ExecutionContext executionContext,
            CancellationTokenSource cancellationTokenSource) => Props.FromProducer(() => new StreamProcessorActor(streamProcessorId,
            streamDefinition,
            createEventProcessorFor,
            executionContext,
            new Mock<Streams.IMetricsCollector>().Object,
            processedEvent,
            failedToProcessEvent,
            NullLogger<StreamProcessorActor>.Instance,
            tenant => create_scoped_processor_props,
            tenants,
            tenant => stream_processor_states,
            cancellationTokenSource));
    };

    private Cleanup cleanup = () => { _ = actor_system.ShutdownAsync(); };
}