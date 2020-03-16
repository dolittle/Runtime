// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Represents the configuration for the <see cref="Microservice"/> core.
    /// </summary>
    public class CoreConfiguration
    {
        /// <summary>
        /// Gets or sets the core programming language used.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the entrypoint of the <see cref="Microservice"/>.
        /// </summary>
        public string EntryPoint { get; set; }
    }
}