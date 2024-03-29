// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.given;

public class all_dependencies : for_FailingPartitions.given.an_instance_of_failing_partitions
{
    protected static readonly ProcessingPosition initial_stream_processor_position = new(3,3);
    protected static StreamProcessorId stream_processor_id;
    protected static StreamProcessorState stream_processor_state;
    protected static IReadOnlyList<StreamEvent> eventStream;

    Establish context = () =>
    {
        stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        stream_processor_state = new StreamProcessorState(initial_stream_processor_position, ImmutableDictionary<PartitionId, FailingPartitionState>.Empty, DateTimeOffset.UtcNow);
        stream_processor_state_repository.Persist(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult();
        events_fetcher
            .Setup(_ => _.FetchInPartition(Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
            .Returns<PartitionId, StreamPosition, CancellationToken>((partition, position, _) =>
            {
                var events = eventStream.Skip((int)position.Value).Where(_ => _.Partition == partition);
                if (events != default && events.Any())
                {
                    return Task.FromResult(Try<IEnumerable<StreamEvent>>.Succeeded(events));
                }
                return Task.FromResult(Try<IEnumerable<StreamEvent>>.Failed(new Exception()));
            });
    };
}