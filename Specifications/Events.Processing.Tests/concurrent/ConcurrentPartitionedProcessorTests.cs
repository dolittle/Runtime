// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Channels;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using FluentAssertions;
using Proto;
using static Dolittle.Runtime.Events.Processing.EventHandlers.Actors.ConcurrentPartitionedProcessor;
using Artifact = Dolittle.Runtime.Artifacts.Artifact;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Events.Processing.Tests.concurrent;

public class ConcurrentPartitionedProcessorTests
{
    private static readonly Dolittle.Runtime.Execution.ExecutionContext ExecutionContext = new(MicroserviceId.New(), TenantId.Development, Version.NotSet,
        Environment.Development, CorrelationId.Empty, ActivitySpanId.CreateRandom(), Claims.Empty, CultureInfo.CurrentCulture);

    private static readonly CommittedEvent SomeEvent = new(
        EventLogSequenceNumber.Initial,
        DateTimeOffset.Now,
        "some-partition",
        ExecutionContext,
        Artifact.New(), false, "{}");

    private static readonly StreamEvent FirstStreamEvent = new(SomeEvent, StreamPosition.Start, StreamId.EventLog, new PartitionId("some-partition"), true);

    [Fact]
    public async Task ShouldReturnNoActionWhenNoInput()
    {
        var state = new State(StreamProcessorState.New, NoWaitingReceipts());

        using var cancellationTokenSource = new CancellationTokenSource(100);
        await state.Invoking(async _ => { await WaitForNextAction(ChannelWithoutEvents(), state, cancellationTokenSource!.Token); }).Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ShouldReturnProcessNextEventWhenMessageAvailable()
    {
        var state = new State(StreamProcessorState.New, NoWaitingReceipts());

        var (nextAction, _) = await WaitForNextAction(ChannelWithEvent(), state, CancellationTokens.FromSeconds(1));

        nextAction.Should().Be(NextAction.ProcessNextEvent);
    }

    [Fact]
    public async Task ShouldReturnRetryFailedActionWhenRetryAvailable()
    {
        var state = new State(FailingProcessorStateWithRetryIn(TimeSpan.Zero, "failing-partition"),
            NoWaitingReceipts());

        var (nextAction, _) = await WaitForNextAction(ChannelWithEvent(), state, CancellationTokens.FromSeconds(1));

        nextAction.Should().Be(NextAction.ProcessFailedEvent);
    }

    [Fact]
    public async Task ShouldReturnProcessNextWhenRetryNotAvailableYet()
    {
        var state = new State(FailingProcessorStateWithRetryIn(TimeSpan.FromSeconds(1), "failing-partition"),
            NoWaitingReceipts());

        var (nextAction, _) = await WaitForNextAction(ChannelWithEvent(), state, CancellationTokens.FromSeconds(1));

        nextAction.Should().Be(NextAction.ProcessNextEvent);
    }

    [Fact]
    public async Task ShouldReturnProcessNextWhenWaitingForReceipt()
    {
        var state = new State(FailingProcessorStateWithRetryIn(TimeSpan.FromSeconds(1), "failing-partition"),
            NoWaitingReceipts());

        var (nextAction, _) = await WaitForNextAction(ChannelWithEvent(), state, CancellationTokens.FromSeconds(1));

        nextAction.Should().Be(NextAction.ProcessNextEvent);
    }

    [Fact]
    public async Task ShouldReturnReceiveResultWhenReceiptReady()
    {
        var state = new State(FailingProcessorStateWithRetryIn(TimeSpan.FromSeconds(1), "failing-partition"),
            WithWaitingReceipt());

        var (nextAction, _) = await WaitForNextAction(ChannelWithEvent(), state, CancellationTokens.FromSeconds(1));

        nextAction.Should().Be(NextAction.ReceiveResult);
    }

    [Fact]
    public async Task ShouldReturnReceiveResultWhenActiveRequestsCompletedAndNoRetryAndNoMessageAvailable()
    {
        var state = new State(StreamProcessorState.New, WithWaitingReceipt());

        var (nextAction, partitionId) = await WaitForNextAction(ChannelWithoutEvents(), state, CancellationTokens.FromSeconds(1));

        nextAction.Should().Be(NextAction.ReceiveResult);
        partitionId.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnProcessFailedEventWhenRetryTimeExpiredAndMessageAvailable()
    {
        var state = new State(FailingProcessorStateWithRetryIn(TimeSpan.Zero, "failing-partition"), NoWaitingReceipts());

        var (nextAction, partitionId) = await WaitForNextAction(ChannelWithEvent(), state, CancellationTokens.FromSeconds(1));

        nextAction.Should().Be(NextAction.ProcessFailedEvent);
        partitionId.Should().Be(new PartitionId("failing-partition"));
    }

    [Fact]
    public async Task ShouldReturnProcessNextEventWhenRetryTimeNotExpiredAndMessageAvailable()
    {
        var state = new State(FailingProcessorStateWithRetryIn(TimeSpan.FromSeconds(1), "failing-partition"), NoWaitingReceipts());

        var (nextAction, partitionId) = await WaitForNextAction(ChannelWithEvent(), state, CancellationTokens.FromSeconds(2));

        nextAction.Should().Be(NextAction.ProcessNextEvent);
        partitionId.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnProcessNextEventWhenRetryTimeNotExpiredAndMessageAvailableAfterDelay()
    {
        var state = new State(FailingProcessorStateWithRetryIn(TimeSpan.FromSeconds(1), "failing-partition"), NoWaitingReceipts());

        var (nextAction, partitionId) =
            await WaitForNextAction(ChannelWithEventAvailableAfter(TimeSpan.FromMicroseconds(50)), state, CancellationTokens.FromSeconds(2));

        nextAction.Should().Be(NextAction.ProcessNextEvent);
        partitionId.Should().BeNull();
    }

    private static ActiveRequests NoWaitingReceipts() => new(5);

    private static ActiveRequests WithWaitingReceipt()
    {
        var activeRequests = new ActiveRequests(5);
        var completedResult = Task.FromResult<ReceiveResult>(current => current);
        activeRequests.Add("waiting-partition", completedResult);
        return activeRequests;
    }

    private static StreamProcessorState FailingProcessorStateWithRetryIn(TimeSpan timeSpan, PartitionId partitionId)
    {
        var processingPosition = new ProcessingPosition(5, 5);
        var failingPosition = new ProcessingPosition(3, 3);

        var retryTime = DateTimeOffset.UtcNow.Add(timeSpan);

        var failingPartition = new FailingPartitionState(failingPosition, retryTime, "#reasons", 1, DateTimeOffset.Now - TimeSpan.FromSeconds(10));

        return new StreamProcessorState(processingPosition, new Dictionary<PartitionId, FailingPartitionState>()
        {
            { partitionId, failingPartition }
        }.ToImmutableDictionary(), DateTimeOffset.UtcNow);
    }

    private static Channel<StreamSubscriptionMessage> ChannelWithEvent()
    {
        var events = Channel.CreateBounded<StreamSubscriptionMessage>(100);
        events.Writer.WriteAsync(new StreamSubscriptionMessage(FirstStreamEvent)).GetAwaiter().GetResult();
        return events;
    }

    private static Channel<StreamSubscriptionMessage> ChannelWithEventAvailableAfter(TimeSpan timeSpan)
    {
        var events = Channel.CreateBounded<StreamSubscriptionMessage>(100);
        Task.Run(async () =>
        {
            await Task.Delay(timeSpan);
            events.Writer.WriteAsync(new StreamSubscriptionMessage(FirstStreamEvent)).GetAwaiter().GetResult();
        });
        return events;
    }

    private static Channel<StreamSubscriptionMessage> ChannelWithoutEvents() => Channel.CreateBounded<StreamSubscriptionMessage>(100);
}