using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines a repository for <see cref="IStreamProcessorState"/>.
/// </summary>
public interface ISubscriptionStateRepository : IStreamProcessorStateRepository<SubscriptionId, StreamProcessorState>
{
}