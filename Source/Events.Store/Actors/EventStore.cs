// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Proto;
using Failure = Dolittle.Runtime.Protobuf.Failure;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Represents the event store grain. 
/// </summary>
[TenantGrain(typeof(EventStoreActor), typeof(EventStoreClient))]
public class EventStore : EventStoreBase
{
    TaskCompletionSource<Try> _nextBatchResult;

    Task<Try> OnNextBatchCompleted => _nextBatchResult.Task;
    CommitBuilder CommitBuilder { get; set; }

    readonly IPersistCommits _commits;
    readonly IFetchCommittedEvents _committedEvents;
    readonly ICreateProps _propsCreator;

    EventLogSequenceNumber _nextSequenceNumber;
    bool _readyToSend = true;
    PID _aggregatesActor;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStore"/> class.
    /// </summary>
    /// <param name="context">The actor context.</param>
    /// <param name="commits">The <see cref="IPersistCommits"/>.</param>
    /// <param name="committedEvents">The <see cref="IFetchCommittedEvents"/> for getting the next event log sequence number.</param>
    public EventStore(IContext context, IPersistCommits commits, IFetchCommittedEvents committedEvents, ICreateProps propsCreator)
        : base(context)
    {
        _commits = commits;
        _committedEvents = committedEvents;
        _propsCreator = propsCreator;
    }

    /// <inheritdoc />
    public override async Task OnStarted()
    {
        // TODO: setup lifecycle hooks, enabling graceful shutdown
        _nextSequenceNumber = await GetNextEventLogSequenceNumber(Context.CancellationToken).ConfigureAwait(false);
        ResetBatchBuilderState(_nextSequenceNumber);
        _aggregatesActor = Context.SpawnNamed(_propsCreator.PropsFor<Aggregates.Aggregates>(), "aggregates");
    }

    /// <inheritdoc />
    public override Task Commit(CommitEventsRequest request, Action<CommitEventsResponse> respond, Action<string> onError)
    {
        var tryAdd = CommitBuilder.TryAddEventsFrom(request);

        if (!tryAdd.Success)
        {
            respond(new CommitEventsResponse
            {
                Failure = tryAdd.Exception.ToProtobuf()
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
        var getAggregateRootVersion = Context.RequestAsync<AggregateRootVersion>(
            _aggregatesActor,
            Aggregates.Aggregates.GetVersion(request.Events.EventSourceId, request.Events.AggregateRootId.ToGuid()));
        Context.ReenterAfter(getAggregateRootVersion, getAggregateRootVersionTask =>
        {
            if (!getAggregateRootVersionTask.IsCompletedSuccessfully || getAggregateRootVersionTask.Result != request.Events.ExpectedAggregateRootVersion)
            {
                // TODO: Respond with concurrency conflict failure
            }
            return Task.CompletedTask;
        });
        var tryAdd = CommitBuilder.TryAddEventsFrom(request);

        if (!tryAdd.Success)
        {
            respond(new CommitAggregateEventsResponse
            {
                Failure = tryAdd.Exception.ToProtobuf()
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
                respond(new CommitAggregateEventsResponse
                {
                    Failure = failure
                });
                return Task.CompletedTask;
            }
            
            respond(new CommitAggregateEventsResponse
            {
                Events = committedEvents.ToProtobuf()
            });
            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request)
    {
        throw new NotImplementedException("Unused");
    }

    /// <inheritdoc />
    public override Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request)
    {
        throw new NotImplementedException("Unused");
    }

    void TrySendBatch()
    {
        if (!_readyToSend || !CommitBuilder.HasCommits)
        {
            return;
        }

        _readyToSend = false;
        var beforeSequenceNumber = _nextSequenceNumber;
        var (commit, newNextSequenceNumber) = CommitBuilder.Build();
        _nextSequenceNumber = newNextSequenceNumber;
        var tcs = _nextBatchResult;
        
        // Reset for next batch
        ResetBatchBuilderState(newNextSequenceNumber);

        Context.ReenterAfter(_commits.Persist(commit, CancellationToken.None), persistTask =>
        {
            _readyToSend = true;
            var completedSuccessfully = persistTask.IsCompletedSuccessfully;

            if (completedSuccessfully)
            {
                tcs.SetResult(Try.Succeeded());
                TrySendBatch();
            }
            else
            {
                // Current version also fails the batch being built
                var failed = Try.Failed(persistTask.Exception);
                _nextBatchResult!.SetResult(failed);
                tcs.SetResult(failed);
                ResetBatchBuilderState(beforeSequenceNumber);
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

    static bool TryGetFailure(Task<Try> task, out Failure failure)
    {
        if (!task.IsCompletedSuccessfully)
        {
            failure = task.Exception!.ToProtobuf();
            return true;
        }

        if (!task.Result.Success)
        {
            failure = task.Result.Exception.ToProtobuf();
            return true;
        }

        failure = default;
        return false;
    }

}
