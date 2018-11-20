using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon.given
{
    public class an_event_horizon : all_dependencies
    {
        protected static EventHorizon event_horizon;
        Establish context = () => event_horizon = new EventHorizon(execution_context_manager.Object, unproccessed_commits_fetcher.Object,logger.Object);
    }
}