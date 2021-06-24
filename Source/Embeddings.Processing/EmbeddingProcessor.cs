// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingProcessor"/>.
    /// </summary>
    public class EmbeddingProcessor : IEmbeddingProcessor
    {
        readonly ConcurrentQueue<Func<bool, Task>> _jobs = new();
        readonly EmbeddingId _embedding;
        readonly IUpdateEmbeddingStates _stateUpdater;
        readonly IStreamEventWatcher _eventWaiter;
        readonly IEventStore _eventStore;
        readonly IEmbeddingStore _embeddingStore;
        readonly ICalculateStateTransitionEvents _transitionCalculator;
        CancellationToken _cancellationToken;
        CancellationTokenSource _waitForEvent;
        bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingProcessor"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> that identifies the embedding.</param>
        /// <param name="stateUpdater">The <see cref="IUpdateEmbeddingStates"/> to use for recalculating embedding states from aggregate root events.</param>
        /// <param name="eventWaiter">The <see cref="IStreamEventWatcher"/> to use for waiting on events to be committed to the event log.</param>
        /// <param name="eventStore">The <see cref="IEventStore"/> to use for fetching and committing aggregate root events.</param>
        /// <param name="embeddingStore">The <see cref="IEmbeddingStore"/> to use for fetching, replacing and removing embedding states.</param>
        /// <param name="transitionCalculator">The <see cref="ICalculateStateTransitionEvents"/> to use for calculating state transition events.</param>
        public EmbeddingProcessor(
            EmbeddingId embedding,
            IUpdateEmbeddingStates stateUpdater,
            IStreamEventWatcher eventWaiter,
            IEventStore eventStore,
            IEmbeddingStore embeddingStore,
            ICalculateStateTransitionEvents transitionCalculator)
        {
            _embedding = embedding;
            _stateUpdater = stateUpdater;
            _eventWaiter = eventWaiter;
            _eventStore = eventStore;
            _embeddingStore = embeddingStore;
            _transitionCalculator = transitionCalculator;
        }

        /// <inheritdoc/>
        public Task<Try> Start(CancellationToken cancellationToken)
        {
            if (HasStarted())
            {
                return Task.FromResult<Try>(new EmbeddingProcessorAlreadyStarted(_embedding));
            }
            _started = true;
            _cancellationToken = cancellationToken;
            return Task.Run(Loop, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Try<ProjectionState>> Update(ProjectionKey key, ProjectionState state, CancellationToken cancellationToken)
            => (await ScheduleJob(() => DoWork(
                    key,
                    currentState => _transitionCalculator.TryConverge(currentState, state, cancellationToken),
                    aggregateRootVersion => _embeddingStore.TryReplace(_embedding, key, aggregateRootVersion, state, cancellationToken),
                    cancellationToken)).ConfigureAwait(false)).With(state);

        /// <inheritdoc/>
        public Task<Try> Delete(ProjectionKey key, CancellationToken cancellationToken)
            => ScheduleJob(() => DoWork(
                key,
                currentState => _transitionCalculator.TryDelete(currentState, cancellationToken),
                aggregateRootVersion => _embeddingStore.TryRemove(_embedding, key, aggregateRootVersion, cancellationToken),
                cancellationToken));

        async Task<Try> Loop()
        {
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var tryRefresh = await EnsureAllStatesAreFresh().ConfigureAwait(false);
                    if (!tryRefresh.Success)
                    {
                        return tryRefresh;
                    }

                    await WaitForEventOrJob().ConfigureAwait(false);
                    await PerformNextJobs().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                await CancelRemainingJobs().ConfigureAwait(false);
            }
            return Try.Succeeded();
        }

        async Task<Try> DoWork(
            ProjectionKey key,
            Func<EmbeddingCurrentState, Task<Try<UncommittedAggregateEvents>>> getUncommittedEvents,
            Func<AggregateRootVersion, Task<Try>> replaceOrRemoveEmbedding,
            CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return new OperationCanceledException();
                }

                var currentState = await _embeddingStore.TryGet(_embedding, key, cancellationToken).ConfigureAwait(false);
                if (!currentState.Success)
                {
                    return currentState.Exception;
                }
                var uncommittedEvents = await getUncommittedEvents(currentState.Result).ConfigureAwait(false);
                if (!uncommittedEvents.Success)
                {
                    return uncommittedEvents.Exception;
                }
                var committedEvents = await _eventStore.CommitAggregateEvents(uncommittedEvents.Result, cancellationToken).ConfigureAwait(false);

                return await replaceOrRemoveEmbedding(committedEvents.Last().AggregateRootVersion + 1).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        async Task WaitForEventOrJob()
        {
            _waitForEvent?.Dispose();
            _waitForEvent = null;
            if (_jobs.TryPeek(out var _))
            {
                return;
            }
            _waitForEvent = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
            try
            {
                await _eventWaiter.WaitForEvent(ScopeId.Default, StreamId.EventLog, _waitForEvent.Token);
            }
            catch (TaskCanceledException)
            {
                // An job has been scheduled
            }
        }

        Task<Try> ScheduleJob(Func<Task<Try>> job)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                return Task.FromResult<Try>(new OperationCanceledException());
            }

            var completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
            _jobs.Enqueue(async (shouldRun) =>
            {
                if (!shouldRun || _cancellationToken.IsCancellationRequested)
                {
                    completionSource.SetResult(new OperationCanceledException());
                }
                else
                {
                    var result = await job().ConfigureAwait(false);
                    completionSource.SetResult(result);
                }
            });
            _waitForEvent?.Cancel();
            return completionSource.Task;
        }

        async Task PerformNextJobs()
        {
            while (_jobs.TryDequeue(out var job))
            {
                await job(true).ConfigureAwait(false);
            }
        }

        async Task CancelRemainingJobs()
        {
            while (_jobs.TryDequeue(out var job))
            {
                await job(false).ConfigureAwait(false);
            }
        }

        Task<Try> EnsureAllStatesAreFresh()
            => _stateUpdater.TryUpdateAll(_cancellationToken);

        bool HasStarted() => _started;
    }
}
