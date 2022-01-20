// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Security;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using Environment = Dolittle.Runtime.Execution.Environment;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using ReverseCallDispatcherType = Dolittle.Runtime.Services.IReverseCallDispatcher<
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
                                    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;
using Version = Dolittle.Runtime.Versioning.Version;
using static Moq.It;
using Dolittle.Runtime.Events.Processing.Contracts;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler.given;

public class all_dependencies
{
    protected static Mock<IStreamProcessors> stream_processors;
    protected static Mock<IValidateFilterForAllTenants> filter_validation;
    protected static Mock<IStreamDefinitions> stream_definitions;
    protected static Mock<ReverseCallDispatcherType> reverse_call_dispatcher;
    protected static Mock<IWriteEventsToStreams> stream_writer;
    protected static EventHandlerRegistrationArguments arguments;
    protected static ILoggerFactory logger_factory;
    protected static FactoryFor<IWriteEventsToStreams> factory_for_stream_writer;
    protected static CancellationToken cancellation_token;
    protected static ExecutionContext execution_context;
    protected static EventProcessorId event_handler_id = Guid.Parse("6afafdf6-33e8-4135-b271-2f7634b70a7f");
    protected static ScopeId scope = Guid.Empty;
    protected static MicroserviceId MicroserviceId = Guid.Parse("cfa85cf2-b463-4e05-8877-211fd3d1237a");
    protected static TenantId tenant = TenantId.Development;
    protected static Version version = Version.NotSet;
    protected static Environment environment = Environment.Production;
    protected static CorrelationId correlation_id = Guid.Parse("720040db-3a60-4755-8d77-31ba4895be46");
    protected static Claims claims = Claims.Empty;
    protected static CultureInfo culture_info = CultureInfo.InvariantCulture;

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
        logger_factory = new NullLoggerFactory();
        execution_context = new ExecutionContext(
            MicroserviceId,
            tenant,
            version,
            environment,
            correlation_id,
            claims,
            culture_info);

        arguments = new EventHandlerRegistrationArguments(
            execution_context,
            event_handler_id,
            Array.Empty<ArtifactId>(),
            false,
            scope,
            "alias");

        factory_for_stream_writer = () => stream_writer.Object;
    };
}