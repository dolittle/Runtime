using Polly;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Defines a system that knows about policies used in <see cref="Subscription"/>.
/// </summary>
public interface ISubscriptionPolicies
{
    /// <summary>
    /// The policy to use for connecting a subscription
    /// </summary>
    IAsyncPolicy Connecting { get; }
}
