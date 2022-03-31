using System;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Proto.Cluster;

namespace Dolittle.Runtime.Actors;

public class TenantGrains : ICanAddTenantServicesForTypesWith<TenantGrainAttribute>
{
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
}
