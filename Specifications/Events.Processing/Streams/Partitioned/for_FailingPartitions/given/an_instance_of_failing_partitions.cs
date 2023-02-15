// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.given;

public class an_instance_of_failing_partitions : all_dependencies
{
    protected static IFailingPartitions failing_partitions;

    Establish context = () => failing_partitions = new FailingPartitions(
        stream_processor_state_repository.Object,
        event_processor.Object,
        events_fetcher.Object,
        create_execution_context);
}