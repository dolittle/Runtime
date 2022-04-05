// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

public class EventStore : EventStoreBase
{
    record CommitRequest(PID Sender, CommitEventsRequest CommitEvents = default, CommitAggregateEventsRequest CommitAggregateEvents = default);

    readonly Dictionary<PID, TaskCompletionSource<CommitEventsResponse>> _commitEventsRequests = new();
    readonly Dictionary<PID, TaskCompletionSource<CommitAggregateEventsResponse>> _commitAggregateEventsRequests = new();

    TaskCompletionSource<Try> _nextBatchResult;

    Task<Try> OnNextBatchCompleted
    {
        get
        {
            _nextBatchResult ??= new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
            return _nextBatchResult.Task;
        }
    }

    CommitBuilder _commitBuilder;

    CommitBuilder CommitBuilder
    {
        get => _commitBuilder ??= new CommitBuilder(_nextSequenceNumber);
        set => _commitBuilder = value;
    }

    readonly List<CommitRequest> _batchedRequests = new();

    readonly IPersistCommits _commits;

    EventLogSequenceNumber _nextSequenceNumber;
    bool _readyToSend = true;

    public EventStore(IContext context, IPersistCommits commits)
        : base(context)
    {
        _commits = commits;
    }

    public override async Task OnStarted()
    {
        _nextSequenceNumber = await GetNextEventLogSequenceNumber(Context.CancellationToken).ConfigureAwait(false);
    }

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

        if (_readyToSend)
        {
            SendBatch();
        }

        Context.ReenterAfter(OnNextBatchCompleted, task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                respond(new CommitEventsResponse
                {
                    Failure = task.Result.Exception.ToProtobuf()
                });
                return Task.CompletedTask;
            }

            if (task.Result.Success)
            {
                respond(new CommitEventsResponse
                {
                    Events = { committedEvents.ToProtobuf() }
                });
                return Task.CompletedTask;
            }

            respond(new CommitEventsResponse
            {
                Failure = task.Result.Exception.ToProtobuf()
            });

            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }

    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request)
    {
        var requestId = Context.Sender!;
        _batchedRequests.Add(new CommitRequest(Context.Sender!, CommitEvents: request));
        _commitEventsRequests[requestId] = new TaskCompletionSource<CommitEventsResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (_readyToSend)
        {
            SendBatch();
        }

        return _commitEventsRequests[requestId].Task;
    }

    public override Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request)
    {
        var sender = Context.Sender;
        _batchedRequests.Add(new CommitRequest(sender, CommitAggregateEvents: request));
        _commitAggregateEventsRequests[sender] = new TaskCompletionSource<CommitAggregateEventsResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (_readyToSend)
        {
            SendBatch();
        }

        return _commitAggregateEventsRequests[sender].Task;
    }

    void SendBatch()
    {
        if (!_batchedRequests.Any())
        {
            _readyToSend = true;
            return;
        }

        _readyToSend = false;
        var batch = _batchedRequests.ToArray();
        _batchedRequests.Clear();

        var commitBuilder = Persistence.Commit.New(_nextSequenceNumber);
        var successfulCommitResponses = new Dictionary<PID, CommitEventsResponse>();
        var successfulCommitAggregateResponses = new Dictionary<PID, CommitAggregateEventsResponse>();
        foreach (var (requestId, commitEventsRequest, commitAggregateEventsRequest) in batch)
        {
            if (commitEventsRequest is not null)
            {
                var addCommit = commitBuilder.TryAddEventsFrom(commitEventsRequest);
                if (!addCommit.Success)
                {
                    _commitEventsRequests[requestId].SetResult(new CommitEventsResponse { Failure = addCommit.Exception.ToProtobuf() });
                }
                else
                {
                    var response = new CommitEventsResponse();
                    response.Events.AddRange(addCommit.Result.ToProtobuf());
                    successfulCommitResponses.Add(requestId, response);
                }
            }

            if (commitAggregateEventsRequest is not null)
            {
                var addCommit = commitBuilder.TryAddEventsFrom(commitAggregateEventsRequest);
                if (!addCommit.Success)
                {
                    _commitAggregateEventsRequests[requestId].SetResult(new CommitAggregateEventsResponse { Failure = addCommit.Exception.ToProtobuf() });
                }
                else
                {
                    successfulCommitAggregateResponses.Add(requestId, new CommitAggregateEventsResponse { Events = addCommit.Result.ToProtobuf() });
                }
            }
        }

        var (commit, newNextSequenceNumber) = commitBuilder.Build();
        Context.ReenterAfter(_commits.Persist(commit, CancellationToken.None), async persist =>
        {
            var result = await persist.ConfigureAwait(false);
            if (!result.Success)
            {
                var failure = result.Exception.ToProtobuf();
                foreach (var request in _commitEventsRequests)
                {
                    request.Value.SetResult(new CommitEventsResponse { Failure = failure });
                }

                foreach (var request in _commitAggregateEventsRequests)
                {
                    request.Value.SetResult(new CommitAggregateEventsResponse { Failure = failure });
                }

                return;
            }

            foreach (var (requestId, taskCompletionSource) in _commitEventsRequests)
            {
                taskCompletionSource.SetResult(successfulCommitResponses[requestId]);
            }

            foreach (var (requestId, taskCompletionSource) in _commitAggregateEventsRequests)
            {
                taskCompletionSource.SetResult(successfulCommitAggregateResponses[requestId]);
            }

            _nextSequenceNumber = newNextSequenceNumber;
        });
    }

    Task<EventLogSequenceNumber> GetNextEventLogSequenceNumber(CancellationToken cancellationToken)
    {
        // TODO: Get from event log
        return Task.FromResult(EventLogSequenceNumber.Initial);
    }
}
