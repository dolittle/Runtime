using Dolittle.Applications;
using Dolittle.Artifacts;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Relativity.for_Singularity.given        
{
    public class a_singularity_with_a_tunnel_and_no_events_to_subscribe_to
    {
        protected static Application application;
        protected static BoundedContext bounded_context;

        protected static Mock<IQuantumTunnel> tunnel;
        
        protected static Singularity singularity;
        protected static EventParticleSubscription subscription;

        Establish context = () => 
        {
            application = Application.New();
            bounded_context = BoundedContext.New();
            subscription = new EventParticleSubscription(new Artifact[0]);
            tunnel = new Mock<IQuantumTunnel>();
            singularity = new Singularity(application, bounded_context, tunnel.Object, subscription);
        };
    }
}