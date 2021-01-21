// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Build
{
    /// <summary>
    /// Defines a system for dealing with configuration for <see cref="IBuildTaskPerformers"/>.
    /// </summary>
    public interface IPerformerConfigurationManager
    {
        /// <summary>
        /// Initializes the system.
        /// </summary>
        /// <param name="jsonFile">JSON file that holds the configuration objects.</param>
        void Initialize(string jsonFile);

        /// <summary>
        /// Get a typed configuration object based on the name of plugin.
        /// </summary>
        /// <param name="configurationType">Type of configuration to get.</param>
        /// <param name="name">Name of plugin.</param>
        /// <returns>A typed instance.</returns>
        object GetFor(Type configurationType, string name);
    }
}