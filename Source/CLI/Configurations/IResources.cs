// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.Rudimentary;
namespace CLI.Configurations
{

    /// <summary>
    /// Defines a system that knows about the resources configuration.
    /// </summary>
    public interface IResources
    {
        /// <summary>
        /// Gets the <see cref="ResourceConfigurationsByTenant"/> configuration object.
        /// </summary>
        /// <param name="configurationName">The name of the resources configuration file.</param>
        /// <returns>The <see cref="ResourceConfigurationsByTenant"/>.</returns>
        Try<ResourceConfigurationsByTenant> TryGet(RuntimeConfigurationName configurationName);
    }
}