// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;
using Machine.Specifications.Utility;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState;

namespace Integration.Tests.Events.Store.MongoDB.for_StreamProcessorStateRepository.for_one_tenant.given;

public static class should_extensions
{
    public static void should_be_the_same_unpartitioned_state_as(this IStreamProcessorState stored_state, IStreamProcessorState expected_state)
    {
        stored_state.ShouldBeOfExactType<StreamProcessorState>();
        
        var stored_unpartitioned_state = (StreamProcessorState)stored_state;
        var expected_unpartitioned_stated = (StreamProcessorState) expected_state;
        
        stored_unpartitioned_state.Partitioned.ShouldBeFalse();
        stored_unpartitioned_state.Position.ShouldEqual(expected_unpartitioned_stated.Position);
        stored_unpartitioned_state.FailureReason.ShouldEqual(expected_unpartitioned_stated.FailureReason);
        stored_unpartitioned_state.IsFailing.ShouldEqual(expected_unpartitioned_stated.IsFailing);
        stored_unpartitioned_state.ProcessingAttempts.ShouldEqual(expected_unpartitioned_stated.ProcessingAttempts);
        stored_unpartitioned_state.LastSuccessfullyProcessed.Should().BeCloseTo(expected_unpartitioned_stated.LastSuccessfullyProcessed, TimeSpan.FromSeconds(1));
        stored_unpartitioned_state.RetryTime.Should().BeCloseTo(expected_unpartitioned_stated.RetryTime, TimeSpan.FromSeconds(1));

    }
    public static void should_be_the_same_partitioned_state_as(this IStreamProcessorState stored_state, IStreamProcessorState expected_state)
    {
        stored_state.ShouldBeOfExactType<Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState>();
        
        var stored_unpartitioned_state = (Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState)stored_state;
        var expected_unpartitioned_stated = (Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState) expected_state;
        
        stored_unpartitioned_state.Partitioned.ShouldBeTrue();
        stored_unpartitioned_state.Position.ShouldEqual(expected_unpartitioned_stated.Position);
        stored_unpartitioned_state.LastSuccessfullyProcessed.Should().BeCloseTo(expected_unpartitioned_stated.LastSuccessfullyProcessed, TimeSpan.FromSeconds(1));
        stored_unpartitioned_state.FailingPartitions.Keys.ShouldContainOnly(expected_unpartitioned_stated.FailingPartitions.Keys);
        stored_unpartitioned_state.FailingPartitions.Each(stored => stored.Value.should_be_the_same_failing_partition_as(expected_unpartitioned_stated.FailingPartitions[stored.Key]));
    }

    public static void should_be_the_same_failing_partition_as(this FailingPartitionState stored, FailingPartitionState expected)
    {
        stored.Reason.ShouldEqual(expected.Reason);
        stored.ProcessingAttempts.ShouldEqual(expected.ProcessingAttempts);
        stored.RetryTime.Should().BeCloseTo(expected.RetryTime, TimeSpan.FromSeconds(1));
        stored.LastFailed.Should().BeCloseTo(expected.LastFailed, TimeSpan.FromSeconds(1));
        stored.Position.ShouldEqual(expected.Position);
    }
}