// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration.DependencyInversion;

/// <summary>
/// Defines a configuration object definition.
/// </summary>
public interface IAmAConfigurationObjectDefinition
{
    /// <summary>
    /// Gets the section where this configuration resides in the <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// </summary>
    public string Section { get; }
    
    /// <summary>
    /// Gets a value indicating whether this Dolittle configuration is per tenant or not.
    /// </summary>
    public bool IsPerTenant { get; }
    
    /// <summary>
    /// Gets the <see cref="Type"/> of the configuration object.
    /// </summary>
    public Type ConfigurationObjectType { get; }
}
