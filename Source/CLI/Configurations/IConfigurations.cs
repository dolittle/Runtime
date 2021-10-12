// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.Rudimentary;
namespace Dolittle.Runtime.CLI.Configurations
{
    /// <summary>
    /// Defines a system that can read configuration files.
    /// </summary>
    public interface IConfigurations
    {
        /// <summary>
        /// Gets the Dolittle Runtime configuration.
        /// </summary>
        /// <param name="configurationName">The name of the configuration file.</param>
        /// <typeparam name="TConfigurationObject">The <see cref="Type"/> of the configuration.</typeparam>
        /// <returns>The parsed configuration object.</returns>
        Try<TConfigurationObject> TryGet<TConfigurationObject>(RuntimeConfigurationName configurationName);
    }

}