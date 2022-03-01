namespace Dolittle.Runtime.EventHorizon.Configuration;

public class EventHorizonConfiguration
{
    public ConsentsPerConsumerConfiguration Consents { get; set; }
    public SubscriptionsPerProducerConfiguration Subscriptions { get; set; }
}
