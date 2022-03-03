using Polly;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Defines a system that knows about policies for <see cref="EventProcessor"/>.
/// </summary>
public interface IEventProcessorPolicies
{
    /// <summary>
    /// Gets the <see cref="IAsyncPolicy"/> for writing events from the event horizon.
    /// </summary>
    IAsyncPolicy WriteEvent { get; }
}