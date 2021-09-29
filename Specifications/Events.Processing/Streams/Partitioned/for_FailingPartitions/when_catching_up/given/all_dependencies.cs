// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.given
{
    public class all_dependencies : for_FailingPartitions.given.an_instance_of_failing_partitions
    {
        protected static readonly StreamPosition initial_stream_processor_position = 3;
        protected static StreamProcessorId stream_processor_id;
        protected static StreamProcessorState stream_processor_state;
        protected static IReadOnlyList<StreamEvent> events;

        Establish context = () =>
        {
            stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            stream_processor_state = new StreamProcessorState(initial_stream_processor_position, new Dictionary<PartitionId, FailingPartitionState>(), DateTimeOffset.UtcNow);

            stream_processor_state_repository
                .Setup(_ => _.TryGetFor(Moq.It.IsAny<IStreamProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<IStreamProcessorState>.Succeeded(stream_processor_state)));

            stream_processor_state_repository
                .Setup(_ => _.Persist(Moq.It.IsAny<IStreamProcessorId>(), Moq.It.IsAny<IStreamProcessorState>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<IStreamProcessorId, IStreamProcessorState, CancellationToken>((stream_processor_id, new_state, _) =>
                    {
                        stream_processor_state = new_state as StreamProcessorState;
                        return Task.FromResult(stream_processor_state);
                    });

            events_fetcher
                .Setup(_ => _.FetchInPartition(Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<PartitionId, StreamPosition, CancellationToken>((partition, position, _) =>
                {
                    var @event = events.FirstOrDefault(_ => _.Position >= position && _.Partition == partition);
                    if (@event != default)
                    {
                        return Task.FromResult(Try<StreamEvent>.Succeeded(@event));
                    }
                    return Task.FromResult(Try<StreamEvent>.Failed(new Exception()));
                });
        };
    }
}