using System.Collections.Generic;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Relativity.for_BootProcedure
{
    public class when_performing_with_two_event_horizons_to_tunnel_through_to
    {
        static Mock<IBarrier> barrier;
        static BootProcedure boot_procedure;

        static Application first_event_horizon_application = Application.New();
        static BoundedContext first_event_horizon_bounded_context = BoundedContext.New();

        static string first_event_horizon_url = "first_url";
        static IEnumerable<Artifact> first_event_horizon_events = new[] {
            new Artifact(ArtifactId.New(), ArtifactGeneration.First),
            new Artifact(ArtifactId.New(), ArtifactGeneration.First),
        };
        
        static Application second_event_horizon_application = Application.New();
        static BoundedContext second_event_horizon_bounded_context = BoundedContext.New();

        static string second_event_horizon_url = "first_url";
        static IEnumerable<Artifact> second_event_horizon_events = new[] {
            new Artifact(ArtifactId.New(), ArtifactGeneration.First),
            new Artifact(ArtifactId.New(), ArtifactGeneration.First),
        };


        Establish context = () => 
        {
            barrier = new Mock<IBarrier>();
            
            var event_horizons_configuration_manager = new Mock<IEventHorizonsConfigurationManager>();
            event_horizons_configuration_manager.SetupGet(_=>_.Current).Returns(
                new EventHorizonsConfiguration {
                    EventHorizons = new[] {
                    new EventHorizonConfiguration {
                        Application = first_event_horizon_application,
                        BoundedContext = first_event_horizon_bounded_context, 
                        Url = first_event_horizon_url,
                        Events = first_event_horizon_events,
                    },
                    new EventHorizonConfiguration {
                        Application = second_event_horizon_application,
                        BoundedContext = second_event_horizon_bounded_context, 
                        Url = second_event_horizon_url,
                        Events = second_event_horizon_events,
                    }
                }
            });

            boot_procedure = new BootProcedure(event_horizons_configuration_manager.Object, barrier.Object);
        };

        Because of = () => boot_procedure.Perform();

        It should_penetrate_barrier_for_first_event_horizon = () => barrier.Verify(_=>
                                                                        _.Penetrate(
                                                                            first_event_horizon_application, 
                                                                            first_event_horizon_bounded_context, 
                                                                            first_event_horizon_url, 
                                                                            first_event_horizon_events), 
                                                                        Moq.Times.Once());

        It should_penetrate_barrier_for_second_event_horizon = () => barrier.Verify(_=>
                                                                        _.Penetrate(
                                                                            second_event_horizon_application, 
                                                                            second_event_horizon_bounded_context, 
                                                                            second_event_horizon_url, 
                                                                            second_event_horizon_events), 
                                                                        Moq.Times.Once());       
    }
}