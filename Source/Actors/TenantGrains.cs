using System;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Proto.Cluster;

namespace Dolittle.Runtime.Actors;

/// <summary>
/// Represents an implementation of <see cref="ICanAddTenantServicesForTypesWith{TAttribute}"/> that adds services for grains with <see cref="TenantGrainAttribute"/>.
/// </summary>
public class TenantGrains : ICanAddTenantServicesForTypesWith<TenantGrainAttribute>, ICanAddServicesForTypesWith<TenantGrainAttribute>
{
    /// <inheritdoc />
    public void AddServiceFor(Type type, TenantGrainAttribute attribute, TenantId tenant, IServiceCollection services)
    {
        services
            .AddTransient(
                attribute.ClientType,
                provider => Activator.CreateInstance(
                    attribute.ClientType,
                    provider.GetRequiredService<Cluster>(),
                    tenant.Value.ToString()))
            .AddTransient(type);
    }

    /// <inheritdoc />
    public void AddServiceFor(Type type, TenantGrainAttribute attribute, IServiceCollection services)
    {
        services
            .AddSingleton(new GrainAndActor(type, attribute.ActorType, true));
    }
}
