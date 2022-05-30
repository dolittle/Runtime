// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    CommitPipeline Pipeline { get; set; }

    readonly IPersistCommits _commits;
    readonly IFetchCommittedEvents _committedEvents;
    readonly IFetchAggregateRootVersions _aggregateRootVersions;
    readonly TenantId _tenantId;
    readonly ICreateProps _propsFactory;
    readonly IApplicationLifecycleHooks _lifecycleHooks;
    readonly Failure _eventStoreShuttingDown = new EventStoreUnavailable("Runtime shutting down").ToFailure();
    
    readonly HashSet<Aggregate> _aggregateCommitInFlight = new();
    readonly Dictionary<Aggregate, AggregateRootVersion> _aggregateRootVersionCache = new();
    readonly Dictionary<ScopeId, PID> _streamSubscriptionManagerPids = new();
    
    bool _readyToSend = true;
    bool _shuttingDown;
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
        _shutdownHook = _lifecycleHooks.RegisterShutdownHook();
        Context.ReenterAfter(_shutdownHook.ShuttingDown, () =>
        {
            _shuttingDown = true;
            if (_readyToSend) // No current batch
            {
                _shutdownHook.MarkCompleted();
            }
        });

        var (eventLogSubscriptionPid, nextSequenceNumber) = await SpawnSubscriptionManager(ScopeId.Default, Context.CancellationToken).ConfigureAwait(false);
        Pipeline = new CommitPipeline(nextSequenceNumber);
        _streamSubscriptionManagerPids[ScopeId.Default] = eventLogSubscriptionPid;
    }

    async Task<(PID pid, EventLogSequenceNumber nextSequenceNumber)> SpawnSubscriptionManager(ScopeId scope, CancellationToken cancellationToken)
    {
        var nextSequenceNumber = await _committedEvents.FetchNextSequenceNumber(scope, cancellationToken).ConfigureAwait(false);  
        var props = _propsFactory.PropsFor<StreamSubscriptionManagerActor>(scope, nextSequenceNumber, _tenantId);
        return (Context.Spawn(props), nextSequenceNumber);
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

        var tryAdd = Pipeline.TryAddEventsFrom(request);

        if (!tryAdd.Success)
        {
            respond(new CommitEventsResponse
            {
                Failure = tryAdd.Exception.ToFailure()
            });
            return Task.CompletedTask;
        }

        var (committedEvents, onNextBatchCompleted) = tryAdd.Result;

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

        var tryAdd = Pipeline.TryAddEventsFrom(request);
        if (!tryAdd.Success)
        {
            return RespondWithFailureAndClearInFlight(tryAdd.Exception.ToFailure());
        }

        var (committedEvents, onNextBatchCompleted) = tryAdd.Result;
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

    public override async Task<CommitExternalEventsResponse> CommitExternal(CommitExternalEventsRequest request)
    {
        var scope = request.ScopeId.ToGuid();
        if (!_streamSubscriptionManagerPids.TryGetValue(scope, out var pid))
        {
            return new CommitExternalEventsResponse{Failure = new Failure($"No active subscription for scope {scope}")};
        }

        IReadOnlyCollection<CommittedEvent> committedEvents = new[]
        {
            request.Event.ToCommittedEvent()
        };
        Context.Send(pid, new Commit(new []
            {
                new CommittedEvents(committedEvents.ToList())
            },
            Array.Empty<CommittedAggregateEvents>(),
            committedEvents,
            request.Event.EventLogSequenceNumber,
            request.Event.EventLogSequenceNumber));

        return new CommitExternalEventsResponse();
    }

    public override async Task RegisterSubscription(EventStoreSubscriptionRequest request, Action<EventStoreSubscriptionAck> respond, Action<string> onError)
    {
        var scope = request.ScopeId.ToGuid();
        if (!_streamSubscriptionManagerPids.TryGetValue(request.ScopeId.ToGuid(), out var pid))
        {
            (pid, _) = await SpawnSubscriptionManager(scope, Context.CancellationToken);
            _streamSubscriptionManagerPids[scope] = pid;
        }
        
        Context.RequestReenter(pid, request, (Task<EventStoreSubscriptionAck> task) =>
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
    }

    public override Task CancelSubscription(CancelEventStoreSubscription request, Action<CancelEventStoreSubscriptionAck> respond, Action<string> onError)
    {
        var scope = request.ScopeId.ToGuid();
        if (!_streamSubscriptionManagerPids.TryGetValue(request.ScopeId.ToGuid(), out var pid))
        {
            onError("Subscription does not exist");
            return Task.CompletedTask;
        }

        Context.RequestReenter(pid, request, (Task<CancelEventStoreSubscriptionAck> task) =>
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

        if (_shuttingDown || !_readyToSend || !Pipeline.TryGetNextCommit(out var commit, out var tcs))
        {
            return;
        }

        _readyToSend = false;

        Context.ReenterAfter(_commits.Persist(commit, CancellationToken.None), persistTask =>
        {
            _readyToSend = true;
            var completedSuccessfully = persistTask.IsCompletedSuccessfully;

            if (completedSuccessfully)
            {
                tcs.SetResult(Try.Succeeded());
                Context.Send(_streamSubscriptionManagerPids[ScopeId.Default], commit);
                TrySendBatch();
            }
            else
            {
                // Current version also fails the batch being built
                var failed = Try.Failed(persistTask.Exception);
                // _nextBatchResult.SetResult(failed); // TODO: What is this? Some leftover code?
                tcs.SetResult(failed);
                FailPipeline(Pipeline, failed);
                Pipeline = new CommitPipeline(commit.FirstSequenceNumber);
            }

            if (_shuttingDown)
            {
                _shutdownHook.MarkCompleted();
            }

            return Task.CompletedTask;
        });
    }

    void FailPipeline(CommitPipeline commitBuilder, Try failure)
    {
        while (commitBuilder.TryGetNextCommit(out _, out var tcs))
        {
            tcs.SetResult(failure);
        }
    }

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
