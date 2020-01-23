// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_HandlerProcessor.given
{
    public class all_dependencies
    {
        protected static readonly int retry_timeout = 123;
        protected static readonly IProcessingResult succeeded_handling_result = new SucceededProcessingResult();
        protected static readonly IProcessingResult failed_handling_result = new FailedProcessingResult();
        protected static readonly IProcessingResult retry_handling_result = new RetryProcessingResult(retry_timeout);

        protected static readonly CommittedEventEnvelope an_event = Processing.given.a_committed_event_envelope;

        protected static EventProcessorId event_processor_id;
        protected static Moq.Mock<IRemoteProcessorService> handler_service_mock;

        Establish context = () =>
        {
            event_processor_id = Guid.NewGuid();
            handler_service_mock = new Moq.Mock<IRemoteProcessorService>();
        };
    }
}