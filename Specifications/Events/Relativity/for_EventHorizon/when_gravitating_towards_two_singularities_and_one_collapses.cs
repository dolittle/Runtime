// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon
{
    public class when_gravitating_towards_two_singularities_and_one_collapses : given.an_event_horizon
    {
        static Mock<ISingularity> first_singularity;
        static Mock<ISingularity> second_singularity;
        static IEnumerable<TenantOffset> tenant_offsets;

        Establish context = () =>
        {
            first_singularity = new Mock<ISingularity>();
            second_singularity = new Mock<ISingularity>();
            tenant_offsets = Array.Empty<TenantOffset>();
            event_horizon.GravitateTowards(first_singularity.Object, tenant_offsets);
            event_horizon.GravitateTowards(second_singularity.Object, tenant_offsets);
        };

        Because of = () => event_horizon.Collapse(first_singularity.Object);

        It should_hold_only_the_remaining_singularity = () => event_horizon.Singularities.ShouldContainOnly(second_singularity.Object);
    }
}