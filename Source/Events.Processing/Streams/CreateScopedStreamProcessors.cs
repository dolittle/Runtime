// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Resilience;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation <see cref="ICreateScopedStreamProcessors" />.
    /// </summary>
    public class CreateScopedStreamProcessors : ICreateScopedStreamProcessors
    {
        readonly IEventFetchers _eventFetchers;
        readonly IResilientStreamProcessorStateRepository _streamProcessorStates;
        readonly IAsyncPolicyFor<ICanFetchEventsFromStream> _eventsFetcherPolicy;
        readonly ILoggerManager _loggerManager;
        readonly IStreamEventWatcher _streamWatcher;
        readonly TenantId _tenant;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateScopedStreamProcessors"/> class.
        /// </summary>
        /// <param name="eventFetchers">The <see cref="IEventFetchers" />.</param>
        /// <param name="streamProcessorStates">The <see cref="IResilientStreamProcessorStateRepository" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="eventsFetcherPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        public CreateScopedStreamProcessors(
            IEventFetchers eventFetchers,
            IResilientStreamProcessorStateRepository streamProcessorStates,
            IExecutionContextManager executionContextManager,
            IAsyncPolicyFor<ICanFetchEventsFromStream> eventsFetcherPolicy,
            IStreamEventWatcher streamWatcher,
            ILoggerManager loggerManager)
        {
            _eventFetchers = eventFetchers;
            _streamProcessorStates = streamProcessorStates;
            _eventsFetcherPolicy = eventsFetcherPolicy;
            _tenant = executionContextManager.Current.Tenant;
            _streamWatcher = streamWatcher;
            _loggerManager = loggerManager;
        }

        /// <inheritdoc />
        public async Task<AbstractScopedStreamProcessor> Create(
            IStreamDefinition streamDefinition,
            IStreamProcessorId streamProcessorId,
            IEventProcessor eventProcessor,
            CancellationToken cancellationToken)
        {
            if (streamDefinition.Partitioned)
            {
                var eventFetcher = await _eventFetchers.GetPartitionedFetcherFor(eventProcessor.Scope, streamDefinition, cancellationToken).ConfigureAwait(false);
                return await CreatePartitionedScopedStreamProcessor(streamProcessorId, streamDefinition, eventProcessor, eventFetcher, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var eventFetcher = await _eventFetchers.GetFetcherFor(eventProcessor.Scope, streamDefinition, cancellationToken).ConfigureAwait(false);
                return await CreateUnpartitionedScopedStreamProcessor(streamProcessorId, streamDefinition, eventProcessor, eventFetcher, cancellationToken).ConfigureAwait(false);
            }
        }

        async Task<Partitioned.ScopedStreamProcessor> CreatePartitionedScopedStreamProcessor(
            IStreamProcessorId streamProcessorId,
            IStreamDefinition streamDefinition,
            IEventProcessor eventProcessor,
            ICanFetchEventsFromPartitionedStream eventsFromStreamsFetcher,
            CancellationToken cancellationToken)
        {
            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(streamProcessorId, cancellationToken).ConfigureAwait(false);
            if (!tryGetStreamProcessorState.Success)
            {
                tryGetStreamProcessorState = Partitioned.StreamProcessorState.New;
                await _streamProcessorStates.Persist(streamProcessorId, tryGetStreamProcessorState.Result, cancellationToken).ConfigureAwait(false);
            }

            if (!tryGetStreamProcessorState.Result.Partitioned) throw new ExpectedPartitionedStreamProcessorState(streamProcessorId);
            NotifyStream(streamProcessorId.ScopeId, streamDefinition, tryGetStreamProcessorState.Result.Position);

            return new Partitioned.ScopedStreamProcessor(
                _tenant,
                streamProcessorId,
                streamDefinition,
                tryGetStreamProcessorState.Result as Partitioned.StreamProcessorState,
                eventProcessor,
                _streamProcessorStates,
                eventsFromStreamsFetcher,
                new Partitioned.FailingPartitions(_streamProcessorStates, eventProcessor, eventsFromStreamsFetcher, _eventsFetcherPolicy, _loggerManager.CreateLogger<Partitioned.FailingPartitions>()),
                _eventsFetcherPolicy,
                _streamWatcher,
                new Partitioned.TimeToRetryForPartitionedStreamProcessor(),
                _loggerManager.CreateLogger<Partitioned.ScopedStreamProcessor>());
        }

        async Task<ScopedStreamProcessor> CreateUnpartitionedScopedStreamProcessor(
            IStreamProcessorId streamProcessorId,
            IStreamDefinition streamDefinition,
            IEventProcessor eventProcessor,
            ICanFetchEventsFromStream eventsFromStreamsFetcher,
            CancellationToken cancellationToken)
        {
            var tryGetStreamProcessorState = await _streamProcessorStates.TryGetFor(streamProcessorId, cancellationToken).ConfigureAwait(false);
            if (!tryGetStreamProcessorState.Success)
            {
                tryGetStreamProcessorState = StreamProcessorState.New;
                await _streamProcessorStates.Persist(streamProcessorId, tryGetStreamProcessorState.Result, cancellationToken).ConfigureAwait(false);
            }

            if (tryGetStreamProcessorState.Result.Partitioned) throw new ExpectedUnpartitionedStreamProcessorState(streamProcessorId);
            NotifyStream(streamProcessorId.ScopeId, streamDefinition, tryGetStreamProcessorState.Result.Position);
            return new ScopedStreamProcessor(
                _tenant,
                streamProcessorId,
                streamDefinition,
                tryGetStreamProcessorState.Result as StreamProcessorState,
                eventProcessor,
                _streamProcessorStates,
                eventsFromStreamsFetcher,
                _eventsFetcherPolicy,
                _streamWatcher,
                new TimeToRetryForUnpartitionedStreamProcessor(),
                _loggerManager.CreateLogger<ScopedStreamProcessor>());
        }

        void NotifyStream(ScopeId scopeId, IStreamDefinition streamDefinition, StreamPosition position)
        {
            if (position == StreamPosition.Start) return;
            if (streamDefinition.Public) _streamWatcher.NotifyForEvent(streamDefinition.StreamId, position - 1);
            else _streamWatcher.NotifyForEvent(scopeId, streamDefinition.StreamId, position - 1);
        }
    }
}
