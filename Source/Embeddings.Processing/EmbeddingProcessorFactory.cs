// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingRequestFactory" />.
    /// </summary>
    [Singleton]
    public class EmbeddingProcessorFactory : IEmbeddingProcessorFactory
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly FactoryFor<IEventStore> _eventStoreFactory;
        readonly FactoryFor<IEmbeddingStore> _embeddingStoreFactory;
        readonly FactoryFor<IStreamEventWatcher> _streamEventWatcherFactory;
        readonly IDetectEmbeddingLoops _embeddingLoopDetector;
        readonly ICompareStates _stateComparer;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes an instance of the <see cref="EmbeddingProcessorFactory" /> class.
        /// </summary>
        public EmbeddingProcessorFactory(
            IExecutionContextManager executionContextManager,
            FactoryFor<IEventStore> eventStoreFactory,
            FactoryFor<IEmbeddingStore> embeddingStoreFactory,
            FactoryFor<IStreamEventWatcher> streamEventWatcherFactory,
            IDetectEmbeddingLoops embeddingLoopDetector,
            ICompareStates stateComparer,
            ILoggerFactory loggerFactory
        )
        {
            _executionContextManager = executionContextManager;
            _eventStoreFactory = eventStoreFactory;
            _embeddingStoreFactory = embeddingStoreFactory;
            _streamEventWatcherFactory = streamEventWatcherFactory;
            _embeddingLoopDetector = embeddingLoopDetector;
            _stateComparer = stateComparer;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc/>
        public IEmbeddingProcessor Create(
            TenantId tenant,
            EmbeddingId embeddingId,
            IEmbedding embedding,
            ProjectionState initialState)
        {
            var oldExecutionContext = _executionContextManager.Current;
            try
            {
                _executionContextManager.CurrentFor(tenant);
                var eventStore = _eventStoreFactory();
                var embeddingStore = _embeddingStoreFactory();
                var projectManyEvents = CreateProjectManyEvents(embeddingId, embedding, initialState);
                return new EmbeddingProcessor(
                    embeddingId,
                    CreateEmbeddingStateUpdater(embeddingId, eventStore, embeddingStore, projectManyEvents),
                    _streamEventWatcherFactory(),
                    eventStore,
                    embeddingStore,
                    CreateStateTransitionEventsCalculator(embeddingId, embedding, projectManyEvents),
                    _loggerFactory.CreateLogger<EmbeddingProcessor>());
            }
            finally
            {
                _executionContextManager.CurrentFor(oldExecutionContext);
            }
        }

        EmbeddingStateUpdater CreateEmbeddingStateUpdater(
            EmbeddingId embeddingId,
            IEventStore eventStore,
            IEmbeddingStore embeddingStore,
            IProjectManyEvents projectManyEvents)
            => new(
                embeddingId,
                eventStore,
                embeddingStore,
                projectManyEvents,
                _loggerFactory.CreateLogger<EmbeddingStateUpdater>());

        StateTransitionEventsCalculator CreateStateTransitionEventsCalculator(
            EmbeddingId embeddingId,
            IEmbedding embedding,
            IProjectManyEvents projectManyEvents)
            => new(
                embeddingId,
                embedding,
                projectManyEvents,
                _stateComparer,
                _embeddingLoopDetector,
                _loggerFactory.CreateLogger<StateTransitionEventsCalculator>());

        ProjectManyEvents CreateProjectManyEvents(
            EmbeddingId embeddingId,
            IEmbedding embedding,
            ProjectionState initialState)
            => new(embeddingId, embedding, initialState, _loggerFactory.CreateLogger<ProjectManyEvents>());
    }
}
