using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors.Aggregates;

[DisableAutoRegistration]
public class AggregateRootInstance : IActor
{
    public class GetAggregateRootVersion
    { }

    public static GetAggregateRootVersion GetVersion() => new(); 
    
    readonly IFetchAggregateRootVersions _aggregateRootVersions;
    readonly ArtifactId _aggregateRootId;
    readonly EventSourceId _eventSourceId;
    
    AggregateRootVersion _version;

    
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootInstance"/> class.
    /// </summary>
    /// <param name="aggregateRootVersions">The <see cref="IFetchAggregateRootVersions"/>.'</param>
    /// <param name="aggregateRootId">The aggregate root <see cref="ArtifactId"/>.</param>
    /// <param name="eventSourceId">The <see cref="EventSourceId"/>.</param>
    public AggregateRootInstance(IFetchAggregateRootVersions aggregateRootVersions, ArtifactId aggregateRootId, EventSourceId eventSourceId)
    {
        _aggregateRootVersions = aggregateRootVersions;
        _aggregateRootId = aggregateRootId;
        _eventSourceId = eventSourceId;
    }

    /// <inheritdoc />
    public async Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case Started:
                _version = await _aggregateRootVersions.FetchVersionFor(_eventSourceId, _aggregateRootId, context.CancellationToken).ConfigureAwait(false); 
                break;
            case CommittedAggregateEvents committedEvents:
                _version = committedEvents.Last().AggregateRootVersion + 1;
                break;
            case GetAggregateRootVersion:
                context.Send(context.Sender!, _version);
                break;
        }
    }
}
