using System;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.DependencyInjection;
using Proto.Cluster;

namespace Dolittle.Runtime.Actors;

public class Grains : ICanAddServicesForTypesWith<GrainAttribute>
{
    public void AddServiceFor(Type type, GrainAttribute attribute, IServiceCollection services)
    {
        services.
            AddTransient(
                attribute.ClientType,
                provider => Activator.CreateInstance(
                    attribute.ClientType,
                    provider.GetRequiredService<Cluster>(),
                    "Groot"))
            .AddTransient(type)
            .AddSingleton(new GrainAndActor(type, attribute.ActorType, false));
    }
}
public class Grains2 : ICanAddServicesForTypesWith<TenantGrainAttribute>
{
    public void AddServiceFor(Type type, TenantGrainAttribute attribute, IServiceCollection services)
    {
        services
            .AddSingleton(new GrainAndActor(type, attribute.ActorType, true));
    }
}
