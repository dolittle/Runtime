// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Represents the configuration for an interaction layer of the <see cref="Microservice"/>.
    /// </summary>
    public class InteractionLayerConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the interaction layer.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the programming language of the interaction layer.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the entrypoint of the interaction layer.
        /// </summary>
        public string EntryPoint { get; set; }

        /// <summary>
        /// Gets or sets the framework of the interaction layer.
        /// </summary>
        public string Framework { get; set; }
    }
}