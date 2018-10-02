using System;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using Moq;
using It=Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon
{
    // public class when_passing_through_with_two_singularities_that_can_receive: given.an_event_horizon_and_a_committed_event_stream
    // {
    //     static Mock<ISingularity>   first_singularity;
    //     static Mock<ISingularity>   second_singularity;

    //     Establish context = () =>
    //     {

    //         first_singularity = new Mock<ISingularity>();
    //         first_singularity.Setup(_ => _.CanPassThrough(committed_event_stream)).Returns(true);
    //         second_singularity = new Mock<ISingularity>();
    //         second_singularity.Setup(_ => _.CanPassThrough(committed_event_stream)).Returns(true);
    //         event_horizon.GravitateTowards(first_singularity.Object);
    //         event_horizon.GravitateTowards(second_singularity.Object);

    //     };

    //     Because of = () => event_horizon.PassThrough(committed_event_stream);

    //     It should_pass_committed_event_stream_to_first_singularity = () => first_singularity.Verify(_ => _.PassThrough(committed_event_stream), Moq.Times.Once());
    //     It should_pass_committed_event_stream_to_second_singularity = () => second_singularity.Verify(_ => _.PassThrough(committed_event_stream), Moq.Times.Once());
    // }
}