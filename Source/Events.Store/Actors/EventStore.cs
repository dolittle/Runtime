// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Proto;
using Aggregate = Dolittle.Runtime.Events.Store.Persistence.Aggregate;
using Failure = Dolittle.Runtime.Protobuf.Failure;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Represents the event store grain. 
/// </summary>
[TenantGrain(typeof(EventStoreActor), typeof(EventStoreClient))]
// ReSharper disable once UnusedType.Global
public class EventStore : EventStoreBase
{
    TaskCompletionSource<Try> _nextBatchResult;

    Task<Try> OnNextBatchCompleted => _nextBatchResult.Task;
    CommitBuilder CommitBuilder { get; set; }

    readonly IPersistCommits _commits;
    readonly IFetchCommittedEvents _committedEvents;
    readonly IFetchAggregateRootVersions _aggregateRootVersions;
    readonly TenantId _tenantId;
    readonly ICreateProps _propsFactory;

    PID _streamSubscriptionManagerPid;

    readonly HashSet<Aggregate> _aggregateCommitInFlight = new();
    readonly Dictionary<Aggregate, AggregateRootVersion> _aggregateRootVersionCache = new();

    EventLogSequenceNumber _nextSequenceNumber;
    bool _readyToSend = true;

    readonly IApplicationLifecycleHooks _lifecycleHooks;

    bool _shuttingDown;
    readonly Failure _eventStoreShuttingDown = new EventStoreUnavailable("Runtime shutting down").ToFailure();
    IShutdownHook _shutdownHook;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStore"/> class.
    /// </summary>
    /// <param name="context">The actor context.</param>
    /// <param name="commits">The <see cref="IPersistCommits"/>.</param>
    /// <param name="committedEvents">The <see cref="IFetchCommittedEvents"/> for getting the next event log sequence number.</param>
    /// <param name="lifecycleHooks"></param>
    /// <param name="aggregateRootVersions"></param>
    /// <param name="tenantId"></param>
    public EventStore(IContext context, IPersistCommits commits, IFetchCommittedEvents committedEvents, IApplicationLifecycleHooks lifecycleHooks,
        IFetchAggregateRootVersions aggregateRootVersions, TenantId tenantId, ICreateProps propsFactory)
        : base(context)
    {
        _commits = commits;
        _committedEvents = committedEvents;
        _lifecycleHooks = lifecycleHooks;
        _aggregateRootVersions = aggregateRootVersions;
        _tenantId = tenantId;
        _propsFactory = propsFactory;
    }

    /// <inheritdoc />
    public override async Task OnStarted()
    {
        _nextSequenceNumber = await GetNextEventLogSequenceNumber(Context.CancellationToken).ConfigureAwait(false);
        ResetBatchBuilderState(_nextSequenceNumber);
        _shutdownHook = _lifecycleHooks.RegisterShutdownHook();
        Context.ReenterAfter(_shutdownHook.ShuttingDown, () =>
        {
            _shuttingDown = true;
            if (_readyToSend) // No current batch
            {
                _shutdownHook.MarkCompleted();
            }
        });

        var propsFor = _propsFactory.PropsFor<StreamSubscriptionManagerActor>(_nextSequenceNumber, _tenantId);
        _streamSubscriptionManagerPid = Context.Spawn(propsFor);
    }

    /// <inheritdoc />
    public override Task Commit(CommitEventsRequest request, Action<CommitEventsResponse> respond, Action<string> onError)
    {
        if (_shuttingDown)
        {
            respond(new CommitEventsResponse
            {
                Failure = _eventStoreShuttingDown
            });
            return Task.CompletedTask;
        }

        var tryAdd = CommitBuilder.TryAddEventsFrom(request);

        if (!tryAdd.Success)
        {
            respond(new CommitEventsResponse
            {
                Failure = tryAdd.Exception.ToFailure()
            });
            return Task.CompletedTask;
        }

        var committedEvents = tryAdd.Result;
        var onNextBatchCompleted = OnNextBatchCompleted;

        TrySendBatch();

        Context.ReenterAfter(onNextBatchCompleted, task =>
        {
            if (TryGetFailure(task, out var failure))
            {
                respond(new CommitEventsResponse
                {
                    Failure = failure
                });
                return Task.CompletedTask;
            }

            respond(new CommitEventsResponse
            {
                Events = { committedEvents.ToProtobuf() }
            });
            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task CommitForAggregate(CommitAggregateEventsRequest request, Action<CommitAggregateEventsResponse> respond, Action<string> onError)
    {
        if (_shuttingDown)
        {
            return RespondWithFailure(_eventStoreShuttingDown);
        }

        var aggregate = new Aggregate(request.Events.AggregateRootId.ToGuid(), request.Events.EventSourceId);

        if (_aggregateCommitInFlight.Contains(aggregate))
        {
            return RespondWithFailure(new EventsForAggregateAlreadyAddedToCommit(aggregate).ToFailure());
        }

        _aggregateCommitInFlight.Add(aggregate);

        if (_aggregateRootVersionCache.TryGetValue(aggregate, out var aggregateRootVersion))
        {
            return CommitForAggregate(request, aggregate, aggregateRootVersion, respond);
        }


        Context.ReenterAfter(GetAggregateRootVersion(aggregate), getAggregateRootVersionTask =>
        {
            if (!getAggregateRootVersionTask.IsCompletedSuccessfully)
            {
                _aggregateCommitInFlight.Remove(aggregate);
                return RespondWithFailure(getAggregateRootVersionTask.Exception.ToFailure());
            }


            var currentAggregateRootVersion = getAggregateRootVersionTask.Result;
            _aggregateRootVersionCache[aggregate] = currentAggregateRootVersion;
            return CommitForAggregate(request, aggregate, currentAggregateRootVersion, respond);
        });
        return Task.CompletedTask;

        Task RespondWithFailure(Failure failure)
        {
            respond(new CommitAggregateEventsResponse { Failure = failure });
            return Task.CompletedTask;
        }
    }

    Task CommitForAggregate(CommitAggregateEventsRequest request, Aggregate aggregate, AggregateRootVersion currentAggregateRootVersion,
        Action<CommitAggregateEventsResponse> respond)
    {
        var expectedAggregateRootVersion = request.Events.ExpectedAggregateRootVersion;
        if (currentAggregateRootVersion != expectedAggregateRootVersion)
        {
            return RespondWithFailureAndClearInFlight(new AggregateRootConcurrencyConflict(
                request.Events.EventSourceId,
                request.Events.AggregateRootId.ToGuid(),
                currentAggregateRootVersion,
                expectedAggregateRootVersion).ToFailure());
        }

        var tryAdd = CommitBuilder.TryAddEventsFrom(request);

        if (!tryAdd.Success)
        {
            return RespondWithFailureAndClearInFlight(tryAdd.Exception.ToFailure());
        }

        var committedEvents = tryAdd.Result;
        var onNextBatchCompleted = OnNextBatchCompleted;

        TrySendBatch();

        Context.ReenterAfter(onNextBatchCompleted, task =>
        {
            if (TryGetFailure(task, out var failure))
            {
                return RespondWithFailureAndClearInFlight(failure);
            }

            _aggregateCommitInFlight.Remove(aggregate);
            _aggregateRootVersionCache[aggregate] = committedEvents[^1].AggregateRootVersion + 1;
            respond(new CommitAggregateEventsResponse
            {
                Events = committedEvents.ToProtobuf()
            });
            return Task.CompletedTask;
        });
        return Task.CompletedTask;

        Task RespondWithFailureAndClearInFlight(Failure failure)
        {
            _aggregateCommitInFlight.Remove(aggregate);
            respond(new CommitAggregateEventsResponse { Failure = failure });
            return Task.CompletedTask;
        }
    }

    Task<AggregateRootVersion> GetAggregateRootVersion(Aggregate aggregate)
    {
        return _aggregateRootVersions.FetchVersionFor(aggregate.EventSourceId, aggregate.AggregateRoot, Context.CancellationToken);
    }

    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request) => throw new NotImplementedException("Unused");

    public override Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request) => throw new NotImplementedException("Unused");


    public override Task RegisterSubscription(EventStoreSubscriptionRequest request, Action<EventStoreSubscriptionAck> respond, Action<string> onError)
    {
        Context.RequestReenter(_streamSubscriptionManagerPid, request, (Task<EventStoreSubscriptionAck> task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    respond(task.Result);
                }
                else
                {
                    onError(task.Exception?.Message ?? "Failed while registering subscription");
                }

                return Task.CompletedTask;
            },
            Context.CancellationToken);
        return Task.CompletedTask;
    }

    public override Task CancelSubscription(CancelEventStoreSubscription request, Action<CancelEventStoreSubscriptionAck> respond, Action<string> onError)
    {
        Context.RequestReenter(_streamSubscriptionManagerPid, request, (Task<CancelEventStoreSubscriptionAck> task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    respond(task.Result);
                }
                else
                {
                    onError(task.Exception?.Message ?? "Failed while cancelling subscription");
                }

                return Task.CompletedTask;
            },
            Context.CancellationToken);
        return Task.CompletedTask;
    }

    
    public override Task<EventStoreSubscriptionAck> RegisterSubscription(EventStoreSubscriptionRequest request) => throw new NotImplementedException("unused");

    public override Task<CancelEventStoreSubscriptionAck> CancelSubscription(CancelEventStoreSubscription request) =>
        throw new NotImplementedException("unused");


    void TrySendBatch()
    {
        // TODO: Refactor

        if (_shuttingDown || !_readyToSend || !CommitBuilder.HasCommits)
        {
            return;
        }

        _readyToSend = false;
        var beforeSequenceNumber = _nextSequenceNumber;
        var (commit, newNextSequenceNumber) = CommitBuilder.Build();
        _nextSequenceNumber = newNextSequenceNumber;
        var tcs = _nextBatchResult;

        ResetBatchBuilderState(newNextSequenceNumber);

        Context.ReenterAfter(_commits.Persist(commit, CancellationToken.None), persistTask =>
        {
            _readyToSend = true;
            var completedSuccessfully = persistTask.IsCompletedSuccessfully;

            if (completedSuccessfully)
            {
                tcs.SetResult(Try.Succeeded());
                Context.Send(_streamSubscriptionManagerPid, commit);
                TrySendBatch();
            }
            else
            {
                // Current version also fails the batch being built
                var failed = Try.Failed(persistTask.Exception);
                _nextBatchResult.SetResult(failed);
                tcs.SetResult(failed);
                ResetBatchBuilderState(beforeSequenceNumber);
            }

            if (_shuttingDown)
            {
                _shutdownHook.MarkCompleted();
            }

            return Task.CompletedTask;
        });
    }

    void ResetBatchBuilderState(EventLogSequenceNumber newNextSequenceNumber)
    {
        CommitBuilder = new CommitBuilder(newNextSequenceNumber);
        _nextBatchResult = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    Task<EventLogSequenceNumber> GetNextEventLogSequenceNumber(CancellationToken cancellationToken)
        => _committedEvents.FetchNextSequenceNumber(cancellationToken);

    static bool TryGetFailure(Task<Try> task, [NotNullWhen(true)] out Failure? failure)
    {
        if (!task.IsCompletedSuccessfully)
        {
            failure = task.Exception!.ToProtobuf();
            return true;
        }

        if (!task.Result.Success)
        {
            failure = task.Result.Exception.ToFailure();
            return true;
        }

        failure = default;
        return false;
    }
}
