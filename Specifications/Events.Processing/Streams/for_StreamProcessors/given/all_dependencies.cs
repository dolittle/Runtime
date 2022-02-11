// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessors.given;

public class all_dependencies
{
    protected static Mock<IPerformActionOnAllTenants> on_all_tenants;
    protected static NullLoggerFactory logger_factory;
    protected static Mock<FactoryFor<ICreateScopedStreamProcessors>> get_scoped_stream_processors_creator;
    protected static Mock<IExecutionContextManager> execution_context_manager;
    protected static IStreamProcessors stream_processors;

    Establish context = () =>
    {
        get_scoped_stream_processors_creator = new Mock<FactoryFor<ICreateScopedStreamProcessors>>();
        on_all_tenants = new Mock<IPerformActionOnAllTenants>();
        logger_factory = new NullLoggerFactory();
        execution_context_manager = new Mock<IExecutionContextManager>();
        stream_processors = new StreamProcessors(
            on_all_tenants.Object,
            get_scoped_stream_processors_creator.Object,
            execution_context_manager.Object,
            logger_factory);
    };
}