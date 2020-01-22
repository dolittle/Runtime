// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.given;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using processors = Dolittle.Runtime.Events.Processing.Specs.given;

namespace Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.for_Register
{
    [Subject(typeof(ScopedEventProcessingHub), nameof(IScopedEventProcessingHub.Register))]
    public class when_registering_a_processor_for_a_tenant_and_artifact_that_is_already_registered : a_scoped_event_processor_hub
    {
        static IEventProcessor processor;
        static Mock<ScopedEventProcessor> already_registered;
        static Mock<ScopedEventProcessor> duplicate;
        static TenantId tenant;

        Establish context = () =>
        {
            tenant = Guid.NewGuid();
            processor = processors.an_event_processor_mock(Artifact.New(), Guid.NewGuid()).Object;
            already_registered = processors.a_scoped_event_processor_mock(tenant, processor);
            duplicate = processors.a_scoped_event_processor_mock(tenant, processor);
            hub.Register(already_registered.Object);
        };

        Because of = () => hub.Register(duplicate.Object);

        It should_replace_the_existing_registration_with_the_new_one = () => hub.GetProcessorsFor(duplicate.Object.Key).ShouldContainOnly(duplicate.Object);
        It should_tell_the_processor_to_catch_up = () => duplicate.Verify(_ => _.CatchUp(), Times.Once());
    }
}