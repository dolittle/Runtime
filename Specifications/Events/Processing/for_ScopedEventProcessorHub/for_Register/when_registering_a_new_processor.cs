// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using processors = Dolittle.Runtime.Events.Specs.Processing.given;

namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.for_Register
{
    [Subject(typeof(ScopedEventProcessingHub), nameof(IStreamProcessingHub.Register))]
    public class when_registering_a_new_processor : a_scoped_event_processor_hub
    {
        static Mock<IEventProcessor> processor;
        static Mock<ScopedEventProcessor> scoped_event_processor;
        static TenantId tenant;

        Establish context = () =>
        {
            tenant = Guid.NewGuid();
            processor = processors.an_event_processor_mock();
            scoped_event_processor = processors.a_scoped_event_processor_mock(tenant, processor.Object);
        };

        Because of = () => hub.Register(scoped_event_processor.Object);

        It should_register_the_event_processor_for_the_tenant_and_event = () => hub.GetProcessorsFor(scoped_event_processor.Object.Key).ShouldContainOnly(new[] { scoped_event_processor.Object });
        It should_tell_the_processor_to_catch_up = () => scoped_event_processor.Verify(_ => _.CatchUp(), Times.Once());
    }
}