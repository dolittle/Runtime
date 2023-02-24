// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.Streams;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents an implementation <see cref="ICreateScopedStreamProcessors" />.
/// </summary>
[Singleton, PerTenant]
public class CreateScopedStreamProcessors : ICreateScopedStreamProcessors
{
    readonly IEventFetchers _eventFetchers;
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly Func<IStreamDefinition, IStreamProcessorId, ICanFetchEventsFromPartitionedStream, IEventProcessor, Partitioned.StreamProcessorState, ExecutionContext, Partitioned.ScopedStreamProcessor> _createPartitionedStreamProcessor;
    readonly Func<IStreamDefinition, IStreamProcessorId, ICanFetchEventsFromStream, IEventProcessor, StreamProcessorState, ExecutionContext, ScopedStreamProcessor> _createUnpartitionedStreamProcessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateScopedStreamProcessors"/> class.
    /// </summary>
    /// <param name="eventFetchers">The factory to use to create the partitioned or unpartitioned stream event fetchers.</param>
    /// <param name="streamProcessorStates">The stream processor states repository to use to get or create the current state of the stream processor.</param>
    /// <param name="streamWatcher">The stream watcher to use to notify waiters when the stream processor is created..</param>
    /// <param name="createPartitionedStreamProcessor">The factory to use to create instances of partitioned stream processors.</param>
    /// <param name="createUnpartitionedStreamProcessor">The factory to use to create instances of unpartitioned stream processors.</param>
    public CreateScopedStreamProcessors(
        IEventFetchers eventFetchers,
        IStreamProcessorStates streamProcessorStates,
        Func<IStreamDefinition, IStreamProcessorId, ICanFetchEventsFromPartitionedStream, IEventProcessor, Partitioned.StreamProcessorState, ExecutionContext, Partitioned.ScopedStreamProcessor> createPartitionedStreamProcessor,
        Func<IStreamDefinition, IStreamProcessorId, ICanFetchEventsFromStream, IEventProcessor, StreamProcessorState, ExecutionContext, ScopedStreamProcessor> createUnpartitionedStreamProcessor)
    {
        _eventFetchers = eventFetchers;
        _streamProcessorStates = streamProcessorStates;
        _createPartitionedStreamProcessor = createPartitionedStreamProcessor;
        _createUnpartitionedStreamProcessor = createUnpartitionedStreamProcessor;
    }

    /// <inheritdoc />
    public async Task<AbstractScopedStreamProcessor> Create(
        IStreamDefinition streamDefinition,
        IStreamProcessorId streamProcessorId,
        IEventProcessor eventProcessor,
        ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        var processorState = await GetOrCreateStreamProcessorState(
                streamProcessorId,
                streamDefinition.Partitioned
                    ? Partitioned.StreamProcessorState.New
                    : StreamProcessorState.New,
                cancellationToken)
            .ConfigureAwait(false);

        AbstractScopedStreamProcessor streamProcessor;
        
        if (streamDefinition.Partitioned)
        {
            if (processorState is not Partitioned.StreamProcessorState partitionedProcessorState)
            {
                throw new ExpectedPartitionedStreamProcessorState(streamProcessorId);
            }
            
            var eventFetcher = await _eventFetchers.GetPartitionedFetcherFor(eventProcessor.Scope, streamDefinition, cancellationToken).ConfigureAwait(false);
            streamProcessor = _createPartitionedStreamProcessor(streamDefinition, streamProcessorId, eventFetcher, eventProcessor, partitionedProcessorState, executionContext);
        }
        else
        {
            if (processorState is not StreamProcessorState unpartitionedProcessorState)
            {
                throw new ExpectedUnpartitionedStreamProcessorState(streamProcessorId);
            }
            
            var eventFetcher = await _eventFetchers.GetFetcherFor(eventProcessor.Scope, streamDefinition, cancellationToken).ConfigureAwait(false);
            streamProcessor = _createUnpartitionedStreamProcessor(streamDefinition, streamProcessorId, eventFetcher, eventProcessor, unpartitionedProcessorState, executionContext);
        }
        
        // NotifyStream(streamProcessorId.ScopeId, streamDefinition, processorState.Position);

        return streamProcessor;
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

    // void NotifyStream(ScopeId scopeId, IStreamDefinition streamDefinition, StreamPosition position)
    // {
    //     if (position == StreamPosition.Start)
    //     {
    //         return;
    //     }
    //     if (streamDefinition.Public)
    //     {
    //         _streamWatcher.NotifyForEvent(streamDefinition.StreamId, position - 1);
    //     }
    //     else
    //     {
    //         _streamWatcher.NotifyForEvent(scopeId, streamDefinition.StreamId, position - 1);
    //     }
    // }
}
