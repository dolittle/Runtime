// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.given
{
    public class an_instance_of_failing_partitions : all_dependencies
    {
        protected static FailingPartitions failing_partitions;

        Establish context = () => failing_partitions = new FailingPartitions(stream_processor_state_repository.Object, events_fetcher.Object, Mock.Of<ILogger>());
    }
}