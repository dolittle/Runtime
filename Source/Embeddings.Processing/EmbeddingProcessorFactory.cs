// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.State;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="IEmbeddingRequestFactory" />.
/// </summary>
[Singleton, PerTenant]
public class EmbeddingProcessorFactory : IEmbeddingProcessorFactory
{
    readonly TenantId _tenant;
    readonly IEventStore _eventStore;
    readonly IEmbeddingStore _embeddingStore;
    readonly IStreamEventWatcher _streamEventWatcher;
    readonly ICompareStates _stateComparer;
    readonly IDetectEmbeddingLoops _loopDetector;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes an instance of the <see cref="EmbeddingProcessorFactory" /> class.
    /// </summary>
    public EmbeddingProcessorFactory(
        TenantId tenant,
        IEventStore eventStore,
        IEmbeddingStore embeddingStore,
        IStreamEventWatcher streamEventWatcher,
        ICompareStates stateComparer,
        IDetectEmbeddingLoops loopDetector,
        ILoggerFactory loggerFactory
    )
    {
        _tenant = tenant;
        _eventStore = eventStore;
        _embeddingStore = embeddingStore;
        _streamEventWatcher = streamEventWatcher;
        _stateComparer = stateComparer;
        _loopDetector = loopDetector;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public IEmbeddingProcessor Create(EmbeddingId embeddingId, IEmbedding embedding, ProjectionState initialState, ExecutionContext executionContext)
    {
        var projectManyEvents = CreateProjectManyEvents(embeddingId, embedding, initialState);
        
        return new EmbeddingProcessor(
            embeddingId,
            _tenant,
            executionContext,
            CreateEmbeddingStateUpdater(embeddingId, projectManyEvents),
            _streamEventWatcher,
            _eventStore,
            _embeddingStore,
            CreateStateTransitionEventsCalculator(embeddingId, embedding, projectManyEvents),
            _loggerFactory.CreateLogger<EmbeddingProcessor>());
    }

    EmbeddingStateUpdater CreateEmbeddingStateUpdater(EmbeddingId embeddingId, IProjectManyEvents projectManyEvents)
        => new(embeddingId, _eventStore, _embeddingStore, projectManyEvents, _loggerFactory.CreateLogger<EmbeddingStateUpdater>());

    StateTransitionEventsCalculator CreateStateTransitionEventsCalculator(EmbeddingId embeddingId, IEmbedding embedding, IProjectManyEvents projectManyEvents)
        => new(embeddingId, embedding, projectManyEvents, _stateComparer, _loopDetector, _loggerFactory.CreateLogger<StateTransitionEventsCalculator>());

    ProjectManyEvents CreateProjectManyEvents(EmbeddingId embeddingId, IEmbedding embedding, ProjectionState initialState)
        => new(embeddingId, embedding, initialState, _loggerFactory.CreateLogger<ProjectManyEvents>());
}
