// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
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

public class Committer : IActor
{
    public record EventLogSubscriptionManagerSpawned(PID Pid);

    readonly TenantId _tenant;
    readonly IPersistCommits _commits;
    readonly IFetchCommittedEvents _committedEvents;
    readonly IFetchAggregateRootVersions _aggregateRootVersions;
    readonly IApplicationLifecycleHooks _lifecycleHooks;

    readonly HashSet<Aggregate> _aggregateCommitInFlight = new();
    readonly Dictionary<Aggregate, AggregateRootVersion> _aggregateRootVersionCache = new();

    PID? _streamSubscriptionManagerPid;
    CommitPipeline? _pipeline;
    IShutdownHook? _shutdownHook;
    
    bool _readyToSend = true;
    bool _shuttingDown = false;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Committer"/> class.
    /// </summary>
    /// <param name="commits">The <see cref="IPersistCommits"/>.</param>
    /// <param name="committedEvents">The <see cref="IFetchCommittedEvents"/>.</param>
    /// <param name="aggregateRootVersions">The <see cref="IFetchAggregateRootVersions"/>.</param>
    /// <param name="lifecycleHooks">The <see cref="IApplicationLifecycleHooks"/>.</param>
    public Committer(
        TenantId tenant,
        IPersistCommits commits,
        IFetchCommittedEvents committedEvents,
        IFetchAggregateRootVersions aggregateRootVersions,
        IApplicationLifecycleHooks lifecycleHooks)
    {
        _tenant = tenant;
        _commits = commits;
        _committedEvents = committedEvents;
        _aggregateRootVersions = aggregateRootVersions;
        _lifecycleHooks = lifecycleHooks;
    }

    /// <inheritdoc />
    public Task ReceiveAsync(IContext context)
        => context.Message switch
        {
            Started => OnStarted(context),
            EventLogSubscriptionManagerSpawned streamSubscriptionManagerPid => OnStreamSubscriptionManagerSet(streamSubscriptionManagerPid),
            CommitEventsRequest request => Commit(context, request, context.Respond),
            CommitAggregateEventsRequest request => CommitForAggregate(context, request, context.Respond),
            _ => Task.CompletedTask
        };

    async Task OnStarted(IContext context)
    {
        _shutdownHook = _lifecycleHooks.RegisterShutdownHook();
        context.ReenterAfter(_shutdownHook.ShuttingDown, () =>
        {
            _shuttingDown = true;
            if (_readyToSend) // No current batch
            {
                _shutdownHook.MarkCompleted();
            }
        });
        var nextSequenceNumber = await _committedEvents.FetchNextSequenceNumber(ScopeId.Default, context.CancellationToken).ConfigureAwait(false);
        _pipeline = CommitPipeline.NewFromEventLogSequenceNumber(nextSequenceNumber);
    }

    Task OnStreamSubscriptionManagerSet(EventLogSubscriptionManagerSpawned msg)
    {
        _streamSubscriptionManagerPid = msg.Pid;
        return Task.CompletedTask;
    }
    
    Task Commit(IContext context, CommitEventsRequest request, Action<CommitEventsResponse> respond)
    {
        if (CannotProcessCommit(out var failure))
        {
            return RespondWithFailure(failure);
        }
        
        if (!_pipeline!.TryAddEventsFrom(request, out var batchedEvents, out var error))
        {
            return RespondWithFailure(error.ToFailure());
        }
        TrySendBatch(context);
        context.ReenterAfter(batchedEvents.Completed, task => TryGetFailure(task, out var failure)
            ? RespondWithFailure(failure)
            : RespondWithEvents(batchedEvents.Item.ToProtobuf()));

        return Task.CompletedTask;

        Task RespondWithEvents(IEnumerable<Contracts.CommittedEvent> events)
        {
            respond(new CommitEventsResponse {Events = {events}});
            return Task.CompletedTask;
        }
        Task RespondWithFailure(Failure failure)
        {
            respond(new CommitEventsResponse { Failure = failure });
            return Task.CompletedTask;
        }
    }

    Task CommitForAggregate(IContext context, CommitAggregateEventsRequest request, Action<CommitAggregateEventsResponse> respond)
    {
        if (CannotProcessCommit(out var failure))
        {
            return RespondWithFailure(failure);
        }

        var aggregate = new Aggregate(request.Events.AggregateRootId.ToGuid(), request.Events.EventSourceId);
        if (_aggregateCommitInFlight.Contains(aggregate))
        {
            return RespondWithFailure(new EventsForAggregateAlreadyAddedToCommit(aggregate).ToFailure());
        }

        _aggregateCommitInFlight.Add(aggregate);
        if (_aggregateRootVersionCache.TryGetValue(aggregate, out var aggregateRootVersion))
        {
            return CommitForAggregate(context, request, aggregate, aggregateRootVersion, respond);
        }
        
        context.ReenterAfter(GetAggregateRootVersion(aggregate, context.CancellationToken), getAggregateRootVersionTask =>
        {
            if (!getAggregateRootVersionTask.IsCompletedSuccessfully)
            {
                _aggregateCommitInFlight.Remove(aggregate);
                return RespondWithFailure(getAggregateRootVersionTask.Exception.ToFailure());
            }

            var currentAggregateRootVersion = getAggregateRootVersionTask.Result;
            _aggregateRootVersionCache[aggregate] = currentAggregateRootVersion;
            return CommitForAggregate(context, request, aggregate, currentAggregateRootVersion, respond);
        });
        return Task.CompletedTask;

        Task RespondWithFailure(Failure failure)
        {
            respond(new CommitAggregateEventsResponse { Failure = failure });
            return Task.CompletedTask;
        }
    }

    Task CommitForAggregate(IContext context, CommitAggregateEventsRequest request, Aggregate aggregate, AggregateRootVersion currentAggregateRootVersion,
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

        if (!_pipeline!.TryAddEventsFrom(request, out var batchedEvents, out var error))
        {
            return RespondWithFailureAndClearInFlight(error.ToFailure());
        }
        
        TrySendBatch(context);
        context.ReenterAfter(batchedEvents.Completed, task =>
        {
            if (TryGetFailure(task, out var failure))
            {
                return RespondWithFailureAndClearInFlight(failure);
            }

            _aggregateCommitInFlight.Remove(aggregate);
            _aggregateRootVersionCache[aggregate] = batchedEvents.Item[^1].AggregateRootVersion + 1;
            var events = batchedEvents.Item.ToProtobuf();
            return RespondWithEvents(events);
        });

        return Task.CompletedTask;

        Task RespondWithEvents(Contracts.CommittedAggregateEvents events)
        {
            respond(new CommitAggregateEventsResponse
            {
                Events = events
            });
            return Task.CompletedTask;
        }
        Task RespondWithFailureAndClearInFlight(Failure failure)
        {
            _aggregateCommitInFlight.Remove(aggregate);
            respond(new CommitAggregateEventsResponse { Failure = failure });
            return Task.CompletedTask;
        }
    }
    
    
    bool CannotProcessCommit(out Dolittle.Protobuf.Contracts.Failure? failure)
    {
        failure = default;
        if (_shuttingDown)
        {
            failure = EventStore.EventStoreShuttingDown;
        }
        else if (_streamSubscriptionManagerPid == default)
        {
            failure = new Dolittle.Protobuf.Contracts.Failure
            {
                Id = FailureId.Other.ToProtobuf(),
                Reason = $"Event Store committer for tenant {_tenant} cannot process commits yet because it has not received the stream subscription manager PID"
            };
        }
        return failure != default;
    }

    void TrySendBatch(IContext context)
    {
        if (_shuttingDown || !_readyToSend || !_pipeline.TryGetNextBatch(out var batchToSend))
        {
            return;
        }

        _readyToSend = false;
        context.ReenterAfter(_commits.Persist(batchToSend.Batch, CancellationToken.None), persistTask =>
        {
            _readyToSend = true;
            if (persistTask.IsCompletedSuccessfully)
            {
                batchToSend.Completion.SetResult(Try.Succeeded());
                context.Send(_streamSubscriptionManagerPid!, batchToSend.Batch);
                TrySendBatch(context);
            }
            else
            {
                batchToSend.Completion.SetResult(persistTask.Exception);
                _pipeline.EmptyAllWithFailure(persistTask.Exception);
                _pipeline = CommitPipeline.NewFromEventLogSequenceNumber(batchToSend.Batch.FirstSequenceNumber);
            }

            if (_shuttingDown)
            {
                _shutdownHook!.MarkCompleted();
            }

            return Task.CompletedTask;
        });
    }

    Task<AggregateRootVersion> GetAggregateRootVersion(Aggregate aggregate, CancellationToken cancellationToken)
        => _aggregateRootVersions.FetchVersionFor(aggregate.EventSourceId, aggregate.AggregateRoot, cancellationToken);
    
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
