// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteEventProcessor.given
{
    public class all_dependencies
    {
        protected static readonly uint retry_timeout = 123;
        protected static readonly PartitionId partition_id = PartitionId.NotSet;
        protected static readonly IProcessingResult succeeded_handling_result = new SucceededProcessingResult();
        protected static readonly IProcessingResult failed_handling_result = new FailedProcessingResult();
        protected static readonly IProcessingResult retry_handling_result = new RetryProcessingResult(retry_timeout);
        protected static readonly Store.CommittedEvent an_event = Processing.given.a_committed_event;

        protected static EventProcessorId event_processor_id;
        protected static Moq.Mock<IRemoteProcessorService> remote_processor_service_mock;

        Establish context = () =>
        {
            event_processor_id = Guid.NewGuid();
            remote_processor_service_mock = Processing.given.a_remote_processor_mock();
        };
    }
}