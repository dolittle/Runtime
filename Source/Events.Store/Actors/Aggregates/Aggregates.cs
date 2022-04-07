using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors.Aggregates;

/// <summary>
/// Represents the an actor that knows about aggregate root instances.
/// </summary>
[DisableAutoRegistration]
public class Aggregates : IActor
{
    public record GetAggregateRootVersion(EventSourceId EventSourceId, ArtifactId AggregateRootId);
    
    public static GetAggregateRootVersion GetVersion(EventSourceId eventSourceId, ArtifactId aggregateRootId) => new(eventSourceId, aggregateRootId); 

    readonly ICreateProps _propsCreator;

    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregates"/> class.
    /// </summary>
    /// <param name="propsCreator">The <see cref="ICreateProps"/>.</param>
    public Aggregates(ICreateProps propsCreator)
    {
        _propsCreator = propsCreator;
    }

    /// <inheritdoc />
    public async Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case GetAggregateRootVersion getAggregateRootVersion: 
                var getVersion = context.RequestAsync<AggregateRootVersion>(
                    GetOrSpawnAggregateRootInstance(
                        context,
                        getAggregateRootVersion.EventSourceId,
                        getAggregateRootVersion.AggregateRootId),
                    AggregateRootInstance.GetVersion());
                var oldContext = context; // TODO: Need this?
                context.ReenterAfter(getVersion, getVersionTask =>
                {
                    if (!getVersionTask.IsCompletedSuccessfully)
                    {
                        // TODO: What here?
                    }
                    oldContext.Send(oldContext.Sender!, getVersionTask.Result);
                    return Task.CompletedTask;
                });
                break;
        }
    }

    PID GetOrSpawnAggregateRootInstance(IContext context, EventSourceId eventSourceId, ArtifactId aggregateRootId)
    {
        var childName = GetAggregateRootInstanceName(eventSourceId, aggregateRootId);
        return context.Children.SingleOrDefault(_ => _.Id.Equals(childName)) ?? context.SpawnNamed(
            _propsCreator.PropsFor<AggregateRootInstance>(eventSourceId, aggregateRootId),
            childName);
    }

    static string GetAggregateRootInstanceName(EventSourceId eventSourceId, ArtifactId aggregateRootId)
        => $"{aggregateRootId.Value}-{eventSourceId.Value}";
}
