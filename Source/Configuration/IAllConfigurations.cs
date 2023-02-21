using System.Collections.Generic;
using Dolittle.Runtime.Domain.Tenancy;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Defines a system that knows about all configurations and their relative Dolittle:Runtime configuration path.
/// </summary>
public interface IAllConfigurations
{
    IReadOnlyDictionary<string, object> Configurations { get; }
    IReadOnlyDictionary<string, IReadOnlyDictionary<TenantId, object>> TenantConfigurations { get; }
}
