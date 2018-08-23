using Dolittle.Execution;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_EventEnvelope
{
    public class when_building_with_new_tcorrelation_id : given.an_event_envelope
    {
        static CorrelationId new_transaction_correlation_id;
        static IEventEnvelope result;

        Establish context = () => new_transaction_correlation_id = CorrelationId.New();

        Because of = () => result = event_envelope.WithTransactionCorrelationId(new_transaction_correlation_id);

        It should_be_a_different_instance = () => result.GetHashCode().ShouldNotEqual(event_envelope.GetHashCode());
        It should_hold_the_new_correlation_id = () => result.CorrelationId.ShouldEqual(new_transaction_correlation_id);
    }
}
