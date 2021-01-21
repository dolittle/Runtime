// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessor.given
{
    public class all_dependencies
    {
        protected static StreamProcessorId stream_processor_id;
        protected static IPerformActionOnAllTenants on_all_tenants;
        protected static IStreamDefinition stream_definition;
        protected static Mock<Func<IEventProcessor>> get_event_processor;
        protected static Mock<FactoryFor<ICreateScopedStreamProcessors>> get_scoped_stream_processors_creator;
        protected static Mock<ICreateScopedStreamProcessors> scoped_stream_processors_creator;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static StreamProcessor stream_processor;
        protected static Mock<ITenants> tenants;

        Establish context = () =>
        {
            stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), StreamId.New());
            stream_definition = new StreamDefinition(new FilterDefinition(Guid.NewGuid(), Guid.NewGuid(), false));
            tenants = new Mock<ITenants>();
            execution_context_manager = new Mock<IExecutionContextManager>();
            scoped_stream_processors_creator = new Mock<ICreateScopedStreamProcessors>();
            get_scoped_stream_processors_creator = new Mock<FactoryFor<ICreateScopedStreamProcessors>>();
            get_scoped_stream_processors_creator.Setup(_ => _.Invoke()).Returns(scoped_stream_processors_creator.Object);
            get_event_processor = new Mock<Func<IEventProcessor>>();
            on_all_tenants = new PerformActionOnAllTenants(tenants.Object, execution_context_manager.Object);
            scoped_stream_processors_creator
                .Setup(_ => _.Create(Moq.It.IsAny<IStreamDefinition>(), Moq.It.IsAny<IStreamProcessorId>(), Moq.It.IsAny<IEventProcessor>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Mock<AbstractScopedStreamProcessor>(
                    new TenantId { Value = Guid.NewGuid() },
                    Mock.Of<IStreamProcessorId>(),
                    Mock.Of<IStreamDefinition>(),
                    Mock.Of<IStreamProcessorState>(),
                    Mock.Of<IEventProcessor>(),
                    Mock.Of<ICanFetchEventsFromStream>(),
                    Mock.Of<IAsyncPolicyFor<ICanFetchEventsFromStream>>(),
                    Mock.Of<IStreamEventWatcher>(),
                    Mock.Of<ILogger>()).Object));
            stream_processor = new StreamProcessor(
                stream_processor_id,
                on_all_tenants,
                stream_definition,
                get_event_processor.Object,
                () => { },
                get_scoped_stream_processors_creator.Object,
                execution_context_manager.Object,
                Mock.Of<ILogger<StreamProcessor>>(),
                CancellationToken.None);
        };
    }
}