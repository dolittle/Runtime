// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="IEmbeddingProcessor"/>.
/// </summary>
[DisableAutoRegistration]
public class EmbeddingProcessor : IEmbeddingProcessor
{
    readonly ConcurrentQueue<Func<bool, Task>> _jobs = new();
    readonly EmbeddingId _embedding;
    readonly TenantId _tenant;
    readonly ExecutionContext _executionContext;
    readonly IUpdateEmbeddingStates _stateUpdater;
    readonly IStreamEventWatcher _eventWaiter;
    readonly IEventStore _eventStore;
    readonly IEmbeddingStore _embeddingStore;
    readonly ICalculateStateTransitionEvents _transitionCalculator;
    readonly ILogger _logger;
    CancellationToken _cancellationToken;
    CancellationTokenSource _waitForEvent;
    bool _started;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingProcessor"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId"/> that identifies the embedding.</param>
    /// <param name="tenant">The <see cref="TenantId"/> that this processor is processing for.</param>
    /// <param name="executionContext">The execution context to run the processor in.</param>
    /// <param name="stateUpdater">The <see cref="IUpdateEmbeddingStates"/> to use for recalculating embedding states from aggregate root events.</param>
    /// <param name="eventWaiter">The <see cref="IStreamEventWatcher"/> to use for waiting on events to be committed to the event log.</param>
    /// <param name="eventStore">The <see cref="IEventStore"/> to use for fetching and committing aggregate root events.</param>
    /// <param name="embeddingStore">The <see cref="IEmbeddingStore"/> to use for fetching, replacing and removing embedding states.</param>
    /// <param name="transitionCalculator">The <see cref="ICalculateStateTransitionEvents"/> to use for calculating state transition events.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public EmbeddingProcessor(
        EmbeddingId embedding,
        TenantId tenant,
        ExecutionContext executionContext,
        IUpdateEmbeddingStates stateUpdater,
        IStreamEventWatcher eventWaiter,
        IEventStore eventStore,
        IEmbeddingStore embeddingStore,
        ICalculateStateTransitionEvents transitionCalculator,
        ILogger logger)
    {
        _embedding = embedding;
        _tenant = tenant;
        _executionContext = CreateExecutionContextForProcessor(executionContext, tenant);
        _stateUpdater = stateUpdater;
        _eventWaiter = eventWaiter;
        _eventStore = eventStore;
        _embeddingStore = embeddingStore;
        _transitionCalculator = transitionCalculator;
        _logger = logger;
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
    public async Task<Try<ProjectionState>> Update(ProjectionKey key, ProjectionState state, ExecutionContext executionContext, CancellationToken cancellationToken)
        => (await ScheduleJob(() => DoWork(
            key,
            currentState => _transitionCalculator.TryConverge(currentState, state, executionContext, cancellationToken),
            aggregateRootVersion => _embeddingStore.TryReplace(_embedding, key, aggregateRootVersion, state, cancellationToken),
            executionContext,
            cancellationToken),
            executionContext)
            .ConfigureAwait(false))
            .With(state);

    /// <inheritdoc/>
    public Task<Try> Delete(ProjectionKey key, ExecutionContext executionContext, CancellationToken cancellationToken)
        => ScheduleJob(() => DoWork(
            key,
            currentState => _transitionCalculator.TryDelete(currentState, executionContext, cancellationToken),
            aggregateRootVersion => _embeddingStore.TryRemove(_embedding, key, aggregateRootVersion, cancellationToken),
            executionContext,
            cancellationToken),
            executionContext);

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
        ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.EventProcessorWorkWasCancelled(_embedding, key);
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
            if (uncommittedEvents.Result.Count <= 0)
            {
                return Try.Succeeded();
            }
            _logger.CommittingTransitionEvents(_embedding, key, uncommittedEvents);
            var committedEvents = await _eventStore.CommitAggregateEvents(uncommittedEvents.Result, executionContext, cancellationToken).ConfigureAwait(false);
            await replaceOrRemoveEmbedding(committedEvents[^1].AggregateRootVersion + 1).ConfigureAwait(false);
            return Try.Succeeded();
        }
        catch (Exception ex)
        {

            _logger.EventProcessorWorkFailed(_embedding, key, ex);
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

    Task<Try> ScheduleJob(Func<Task<Try>> job, ExecutionContext requestExecutionContext)
    {
        if (_cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<Try>(new OperationCanceledException());
        }

        if (requestExecutionContext.Tenant != _tenant)
        {
            throw new EmbeddingRequestWorkScheduledForWrongTenant(requestExecutionContext.Tenant, _tenant);
        }

        var completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
        _jobs.Enqueue(async shouldRun =>
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

    async Task<Try> EnsureAllStatesAreFresh()
    {
        var result = await _stateUpdater.TryUpdateAll(_executionContext, _cancellationToken).ConfigureAwait(false);
        if (!result.Success)
        {
            _logger.FailedEnsuringAllStatesAreFresh(_embedding, result.Exception);
        }
        return result;
    }

    bool HasStarted() => _started;

    static ExecutionContext CreateExecutionContextForProcessor(ExecutionContext registrationContext, TenantId tenant)
        => registrationContext with
        {
            Tenant = tenant,
            // TODO: Does this make sense?
        };
}
