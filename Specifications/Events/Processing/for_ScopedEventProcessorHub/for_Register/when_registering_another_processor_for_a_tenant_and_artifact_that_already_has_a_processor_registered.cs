// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using processors = Dolittle.Runtime.Events.Specs.Processing.given;

namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.for_Register
{
    [Subject(typeof(ScopedEventProcessingHub), nameof(IScopedEventProcessingHub.Register))]
    public class when_registering_another_processor_for_a_tenant_and_artifact_that_already_has_a_processor_registered : a_scoped_event_processor_hub
    {
        static Mock<ScopedEventProcessor> already_registered;
        static Mock<ScopedEventProcessor> registered_on_same_event;
        static TenantId tenant;

        Establish context = () =>
        {
            tenant = Guid.NewGuid();
            var artifact = Artifact.New();
            var first_processor = processors.an_event_processor_mock(artifact, Guid.NewGuid()).Object;
            var second_processor = processors.an_event_processor_mock(artifact, Guid.NewGuid()).Object;
            already_registered = processors.a_scoped_event_processor_mock(tenant, first_processor);
            registered_on_same_event = processors.a_scoped_event_processor_mock(tenant, second_processor);
            hub.Register(already_registered.Object);
        };

        Because of = () => hub.Register(registered_on_same_event.Object);

        It should_add_the_new_registration_alongside_the_old = () => hub.GetProcessorsFor(registered_on_same_event.Object.Key)
                                                                            .ShouldContainOnly(new[] { already_registered.Object, registered_on_same_event.Object });

        It should_tell_the_processor_to_catch_up = () => registered_on_same_event.Verify(_ => _.CatchUp(), Times.Once());
    }
}