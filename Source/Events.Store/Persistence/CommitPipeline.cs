// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Persistence;

class CommitPipeline
{
    const int BatchSize = 1000;

    CommitBuilder _currentBuilder;
    TaskCompletionSource<Try> _completionSource;

    readonly Channel<(Commit, TaskCompletionSource<Try>)> _preparedBatches = Channel.CreateUnbounded<(Commit, TaskCompletionSource<Try>)>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = true
    });

    public CommitPipeline(EventLogSequenceNumber nextSequenceNumber)
    {
        _currentBuilder = new CommitBuilder(nextSequenceNumber);
        _completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public Try<(CommittedEvents,Task<Try>)> TryAddEventsFrom(CommitEventsRequest request)
    {
        var result = _currentBuilder.TryAddEventsFrom(request);
        if (result.Success)
        {
            var tcs = _completionSource;
            ReplaceBatchIfFull();
            return (result, tcs.Task);
        }

        return result.Exception;
    }

    public Try<(CommittedAggregateEvents,Task<Try>)> TryAddEventsFrom(CommitAggregateEventsRequest request)
    {
        var result = _currentBuilder.TryAddEventsFrom(request);
        if (result.Success)
        {
            var tcs = _completionSource;
            ReplaceBatchIfFull();
            return (result, tcs.Task);
        }

        return result.Exception;
    }

    void ReplaceBatchIfFull()
    {
        if (_currentBuilder.Count < BatchSize) return;
        var commitAndTask = BuildAndReplaceCurrentBatch();
        _preparedBatches.Writer.TryWrite(commitAndTask);
    }

    (Commit, TaskCompletionSource<Try>) BuildAndReplaceCurrentBatch()
    {
        var (commit, nextEventLogSequenceNumber) = _currentBuilder.Build();
        var tcs = _completionSource;
        _currentBuilder = new CommitBuilder(nextEventLogSequenceNumber);
        _completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
        return (commit, tcs);
    }

    public bool TryGetNextCommit([NotNullWhen(true)] out Commit commit, [NotNullWhen(true)] out TaskCompletionSource<Try> taskCompletionSource)
    {
        if (_preparedBatches.Reader.TryRead(out var tuple))
        {
            commit = tuple.Item1;
            taskCompletionSource = tuple.Item2;
            return true;
        }

        if (_currentBuilder.HasCommits)
        {
            var (builtCommit, tcs) = BuildAndReplaceCurrentBatch();
            commit = builtCommit;
            taskCompletionSource = tcs;
            return true;
        }

        commit = default;
        taskCompletionSource = default;
        return false;
    }
}
