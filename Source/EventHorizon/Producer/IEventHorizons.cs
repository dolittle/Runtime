
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Defines a system that knows about event horizons.
/// </summary>
public interface IEventHorizons
{
    /// <summary>
    /// Starts an Event Horizon.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="arguments">The connecting arguments.</param>
    /// <param name="consent">The configured consent.</param>
    /// <param name="cancellationToken">Cancellation token that can cancel the event horizon.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous processing operation.</returns>
    Task Start(
        IReverseCallDispatcher<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, SubscriptionResponse, ConsumerRequest, ConsumerResponse> dispatcher,
        ConsumerSubscriptionArguments arguments,
        ConsentId consent,
        CancellationToken cancellationToken);
}
