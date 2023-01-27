// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents an implementation <see cref="ICreateScopedFilterStreamProcessors" />.
/// </summary>
[Singleton, PerTenant]
public class CreateScopedFilterStreamProcessors : ICreateScopedFilterStreamProcessors
{
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly IStreamEventWatcher _streamWatcher;
    readonly IEventLogStream _eventLogStream;
    readonly Func<ChannelReader<EventLogBatch>, StreamProcessorId, bool, StreamProcessorState, ScopedFilterStreamProcessor> _createFilterStreamProcessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateScopedFilterStreamProcessors"/> class.
    /// </summary>
    /// <param name="streamProcessorStates">The stream processor states repository to use to get or create the current state of the stream processor.</param>
    /// <param name="streamWatcher">The stream watcher to use to notify waiters when the stream processor is created..</param>
    /// <param name="eventLogStream">The <see cref="IEventLogStream"/>.</param>
    /// <param name="createFilterStreamProcessor">The factory to use to create instances of unpartitioned stream processors.</param>
    public CreateScopedFilterStreamProcessors(
        IStreamProcessorStates streamProcessorStates,
        IStreamEventWatcher streamWatcher,
        IEventLogStream eventLogStream,
        Func<ChannelReader<EventLogBatch>, StreamProcessorId, bool, StreamProcessorState, ScopedFilterStreamProcessor> createFilterStreamProcessor)
    {
        _streamProcessorStates = streamProcessorStates;
        _streamWatcher = streamWatcher;
        _eventLogStream = eventLogStream;
        _createFilterStreamProcessor = createFilterStreamProcessor;
    }

    /// <inheritdoc />
    public async Task<ScopedFilterStreamProcessor> Create(
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        StreamProcessorId streamProcessorId,
        CancellationToken cancellationToken)
    {
        var processorState = await GetOrCreateStreamProcessorState(
                streamProcessorId, 
                StreamProcessorState.New,
                cancellationToken)
            .ConfigureAwait(false);
        
        if (processorState is not StreamProcessorState unpartitionedProcessorState)
        {
            throw new ExpectedUnpartitionedStreamProcessorState(streamProcessorId);
        }
        // TODO: This is not needed when using the fast event handler.
        NotifyStream(streamProcessorId.ScopeId, filterDefinition, processorState.Position);
        return _createFilterStreamProcessor(
            _eventLogStream.Subscribe(
                streamProcessorId.ScopeId,
                processorState.Position.Value,
                new ReadOnlyCollection<ArtifactId>(filterDefinition.Types.ToList()),
                CancellationToken.None),
            streamProcessorId,
            filterDefinition.Partitioned,
            unpartitionedProcessorState);
    }

    async Task<IStreamProcessorState> GetOrCreateStreamProcessorState(IStreamProcessorId streamProcessorId, IStreamProcessorState initialState, CancellationToken cancellationToken)
    {
        var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(streamProcessorId, cancellationToken).ConfigureAwait(false);

        if (tryGetStreamProcessorState.Success)
        {
            return tryGetStreamProcessorState.Result;
        }
        
        await _streamProcessorStates.Persist(streamProcessorId, initialState, cancellationToken).ConfigureAwait(false);
        return initialState;
    }

    void NotifyStream(ScopeId scopeId, TypeFilterWithEventSourcePartitionDefinition filterDefinition, StreamPosition position)
    {
        if (position == StreamPosition.Start)
        {
            return;
        }
        if (filterDefinition.Public)
        {
            _streamWatcher.NotifyForEvent(filterDefinition.TargetStream, position - 1);
        }
        else
        {
            _streamWatcher.NotifyForEvent(scopeId, filterDefinition.TargetStream, position - 1);
        }
    }
}
