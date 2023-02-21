// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.Configuration;

/// <summary>
/// Defines the Configuration management client.
/// </summary>
public interface IManagementClient
{
    /// <summary>
    /// Gets all registered Aggregate Roots or for a specific Tenant if specified.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<string> GetConfigurationYaml(MicroserviceAddress runtime);

}
