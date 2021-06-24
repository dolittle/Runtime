// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using It = Moq.It;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessorFactory.given
{
    public class all_dependencies
    {
        protected static ExecutionContext execution_context;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<FactoryFor<IEventStore>> factory_for_event_store;
        protected static Mock<FactoryFor<IEmbeddingStore>> factory_for_embedding_store;
        protected static Mock<FactoryFor<IStreamEventWatcher>> factory_for_stream_event_watcher;
        protected static Mock<IConvertProjectionKeysToEventSourceIds> convert_projections_keys_to_event_source_ids;
        protected static Mock<IDetectEmbeddingLoops> detect_embedding_loops;
        protected static Mock<ICompareStates> compare_states;
        protected static EmbeddingProcessorFactory factory;

        Establish context = () =>
        {
            execution_context = new ExecutionContext(
                "bc0ca6f8-09a3-4292-b3f7-42168c34732d",
                "6a3b2a79-bf9b-4b56-aeda-554d2b15f325",
                Versioning.Version.NotSet,
                "env",
                "cf203a09-30eb-4e71-be2f-810ef3529152",
                Security.Claims.Empty,
                System.Globalization.CultureInfo.InvariantCulture);
            execution_context_manager = new Mock<IExecutionContextManager>();
            execution_context_manager.SetupGet(_ => _.Current).Returns(execution_context);
            execution_context_manager
                .Setup(_ => _.CurrentFor(It.IsAny<TenantId>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns<TenantId, string, int, string>((tenant, _, _, _) => execution_context with { Tenant = tenant });
            execution_context_manager
                .Setup(_ => _.CurrentFor(It.IsAny<ExecutionContext>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns<ExecutionContext, string, int, string>((context, _, _, _) => context);
            factory_for_event_store = new Mock<FactoryFor<IEventStore>>();
            factory_for_event_store.Setup(_ => _.Invoke()).Returns(Mock.Of<IEventStore>());
            factory_for_embedding_store = new Mock<FactoryFor<IEmbeddingStore>>();
            factory_for_embedding_store.Setup(_ => _.Invoke()).Returns(Mock.Of<IEmbeddingStore>());
            factory_for_stream_event_watcher = new Mock<FactoryFor<IStreamEventWatcher>>();
            factory_for_stream_event_watcher.Setup(_ => _.Invoke()).Returns(Mock.Of<IStreamEventWatcher>());
            convert_projections_keys_to_event_source_ids = new Mock<IConvertProjectionKeysToEventSourceIds>();
            detect_embedding_loops = new Mock<IDetectEmbeddingLoops>();
            compare_states = new Mock<ICompareStates>();
            factory = new EmbeddingProcessorFactory(
                execution_context_manager.Object,
                factory_for_event_store.Object,
                factory_for_embedding_store.Object,
                factory_for_stream_event_watcher.Object,
                convert_projections_keys_to_event_source_ids.Object,
                detect_embedding_loops.Object,
                compare_states.Object,
                NullLoggerFactory.Instance);
        };
    }
}
