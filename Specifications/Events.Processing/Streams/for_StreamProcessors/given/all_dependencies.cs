// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessors.given;

public class all_dependencies
{
    protected static NullLoggerFactory logger_factory;
    protected static Mock<Func<StreamProcessorId, EventProcessorKind, IStreamDefinition, Action, Func<TenantId, IEventProcessor>, ExecutionContext, CancellationToken, StreamProcessor>> create_stream_processor;
    protected static Mock<ICreateExecutionContexts> execution_context_creator;
    protected static IStreamProcessors stream_processors;

    Establish context = () =>
    {
        create_stream_processor = new Mock<Func<StreamProcessorId, EventProcessorKind, IStreamDefinition, Action, Func<TenantId, IEventProcessor>, ExecutionContext, CancellationToken, StreamProcessor>>();
        logger_factory = new NullLoggerFactory();
        execution_context_creator = new Mock<ICreateExecutionContexts>();
        execution_context_creator
            .Setup(_ => _.TryCreateUsing(Moq.It.IsAny<ExecutionContext>()))
            .Returns<ExecutionContext>(_ => _);
        stream_processors = new StreamProcessors(
            create_stream_processor.Object,
            Mock.Of<IMetricsCollector>(),
            execution_context_creator.Object,
            Mock.Of<ILogger>());
    };
}
