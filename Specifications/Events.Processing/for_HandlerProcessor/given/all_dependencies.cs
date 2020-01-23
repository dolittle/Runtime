// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_HandlerProcessor.given
{
    public class all_dependencies
    {
        protected static readonly IHandlerResult succeeded_handling_result = new succeeded_handling_result();
        protected static readonly IHandlerResult failed_handling_result = new failed_handling_result();
        protected static readonly IHandlerResult retry_handling_result = new retry_handling_result();

        protected static readonly CommittedEventEnvelope an_event = Processing.given.a_committed_event_envelope;

        protected static TenantId tenant_id;
        protected static EventProcessorId event_processor_id;
        protected static Moq.Mock<IHandlerService> handler_service_mock;

        Establish context = () =>
        {
            tenant_id = Guid.NewGuid();
            event_processor_id = Guid.NewGuid();
            handler_service_mock = new Moq.Mock<IHandlerService>();
        };
    }
}