using System.Collections.Generic;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon
{
    public class when_gravitating_towards_two_singularities : given.an_event_horizon
    {
        static Mock<ISingularity>   first_singularity;
        static Mock<ISingularity>   second_singularity;

        static IEnumerable<TenantOffset> tenant_offsets;

        Establish context = () =>
        {
            first_singularity = new Mock<ISingularity>();
            second_singularity = new Mock<ISingularity>();

            tenant_offsets = new TenantOffset[]{};
        };


        Because of = () => 
        {
            event_horizon.GravitateTowards(first_singularity.Object, tenant_offsets);
            event_horizon.GravitateTowards(second_singularity.Object, tenant_offsets);
        };

        It should_hold_only_the_singularities_to_pass_through = () => event_horizon.Singularities.ShouldContainOnly(first_singularity.Object, second_singularity.Object);
    }
}