using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Runtime.Configuration.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Represents an implementation of <see cref="IAllConfigurations"/>;
/// </summary>
[Singleton]
public class AllConfigurations : IAllConfigurations
{
#pragma warning disable CS8618
    public AllConfigurations(IServiceProvider serviceProvider, IEnumerable<IAmAConfigurationObjectDefinition> configurationObjectDefinitions)
#pragma warning restore CS8618
    {
        Populate(serviceProvider, configurationObjectDefinitions.ToArray());
    }

    void Populate(IServiceProvider serviceProvider, IAmAConfigurationObjectDefinition[] configurationObjectDefinitions)
    {
        //TODO: All these dynamics and crazy reflection should be made simpler whenever we merge the projects.
        var configurations = new Dictionary<string, object>();
        var tenantConfigurations = new Dictionary<string, IReadOnlyDictionary<TenantId, object>>();
        
        foreach (var definition in configurationObjectDefinitions.Where(_ => !_.IsPerTenant))
        {
            configurations.Add(definition.Section, GetNonTenantConfiguration(serviceProvider, definition.ConfigurationObjectType));
        }

        var tenantsConfiguration = configurations["tenants"] as IEnumerable;
        
        var tenants = tenantsConfiguration!.Cast<dynamic>().Select(_ => (TenantId)_.Key);

        foreach (var definition in configurationObjectDefinitions.Where(_ => _.IsPerTenant))
        {
            tenantConfigurations.Add(definition.Section, GetTenantConfigurations(serviceProvider, definition.ConfigurationObjectType, tenants));
        }

        Configurations = configurations;
        TenantConfigurations = tenantConfigurations;
    }

    static IReadOnlyDictionary<TenantId, object> GetTenantConfigurations(IServiceProvider serviceProvider, Type configurationType, IEnumerable<TenantId> tenants)
    {
        var result = new Dictionary<TenantId, object>();
        dynamic getOptions = serviceProvider.GetService(typeof(Func<,>).MakeGenericType(typeof(TenantId), typeof(IOptions<>).MakeGenericType(configurationType)))!;
        foreach (var tenant in tenants)
        {
            result.Add(tenant, GetConfigurationValue((object)getOptions.Invoke(tenant), configurationType));
        }
        
        return result.AsReadOnly();
    }

    static object GetNonTenantConfiguration(IServiceProvider serviceProvider, Type configurationType)
    {
        var options = serviceProvider.GetService(typeof(IOptions<>).MakeGenericType(configurationType))!;
        return GetConfigurationValue(options, configurationType);
    }


    static object GetConfigurationValue(object options, Type configurationType)
    {
        var method = typeof(AllConfigurations).GetMethod(nameof(GetValue), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(configurationType);
        return method.Invoke(null, new[] {options})!;
    }
    
    static T GetValue<T>(IOptions<T> options) where T : class => options.Value;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object> Configurations { get; private set; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, IReadOnlyDictionary<TenantId, object>> TenantConfigurations { get; private set; }
}
