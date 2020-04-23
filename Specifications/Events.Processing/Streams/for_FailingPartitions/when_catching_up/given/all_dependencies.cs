// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.when_catching_up.given
{
    public class all_dependencies : for_FailingPartitions.given.an_instance_of_failing_partitions
    {
        protected static readonly StreamPosition initial_stream_processor_position = 3;
        protected static StreamProcessorId stream_processor_id;
        protected static Mock<IEventProcessor> event_processor;
        protected static StreamProcessorState stream_processor_state;
        protected static IReadOnlyList<CommittedEvent> committed_events;

        Establish context = () =>
        {
            committed_events = new ReadOnlyCollection<CommittedEvent>(new CommittedEvent[]
            {
                Processing.committed_events.single(),
                Processing.committed_events.single(),
                Processing.committed_events.single()
            });
            stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            event_processor = new Mock<IEventProcessor>();
            stream_processor_state = new StreamProcessorState(initial_stream_processor_position, new Dictionary<PartitionId, FailingPartitionState>());

            events_fetcher
                .Setup(_ => _.Fetch(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<ScopeId, StreamId, StreamPosition, CancellationToken>((scope, id, pos, _) => Task.FromResult(new StreamEvent(committed_events[(int)pos.Value], pos, Guid.NewGuid(), Guid.NewGuid())));
            stream_processor_state_repository
                .Setup(_ => _.GetOrAddNew(Moq.It.IsAny<StreamProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(stream_processor_state));

            stream_processor_state_repository
                .Setup(_ => _.SetFailingPartitionState(Moq.It.IsAny<StreamProcessorId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<FailingPartitionState>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<StreamProcessorId, PartitionId, FailingPartitionState, CancellationToken>((stream_processor_id, partition, failing_partition_state, _) =>
                {
                    stream_processor_state.FailingPartitions[partition] = failing_partition_state;
                    return Task.FromResult(stream_processor_state);
                });
            stream_processor_state_repository
                .Setup(_ => _.RemoveFailingPartition(Moq.It.IsAny<StreamProcessorId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<StreamProcessorId, PartitionId, CancellationToken>((stream_processor_id, partition, _) =>
                {
                    stream_processor_state.FailingPartitions.Remove(partition);
                    return Task.FromResult(stream_processor_state);
                });
        };
    }
}