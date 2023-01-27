// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up;

public class and_there_are_no_failing_partitions : given.all_dependencies
{
    static IStreamProcessorState result;

    Establish context = () => stream_processor_state.FailingPartitions.Clear();

    Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult();

    It should_return_a_state = () => result.Should().NotBeNull();
    It should_return_a_state_of_the_expected_type = () => result.Should().BeOfType<StreamProcessorState>();
    It should_return_a_state_with_the_same_position = () => result.Position.Should().Be(stream_processor_state.Position);
    It should_return_a_state_with_the_correct_partitioned_value = () => result.Partitioned.Should().Be(stream_processor_state.Partitioned);
    It should_have_no_failing_partitions = () => (result as StreamProcessorState).FailingPartitions.Should().BeEmpty();
    It should_not_process_any_events = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);
}