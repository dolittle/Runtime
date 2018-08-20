using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon
{
    public class when_creating : given.all_dependencies
    {
        static EventHorizon result;
        Because of = () => result = new EventHorizon(lens.Object, logger.Object);
        It should_start_observing_for_the_event_horizon = () => lens.Verify(_ => _.ObserveFor(result), Moq.Times.Once());
    }
}