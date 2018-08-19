/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents the configuration of the server
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Configuration"/>
        /// </summary>
        public Configuration()
        {
            Interaction = new InteractionConfiguration();
            Management = new ManagementConfiguration();
        }

        /// <summary>
        /// Gets or sets the configuration for <see cref="IInteractionServer"/>
        /// </summary>
        public InteractionConfiguration Interaction { get; set; }

        /// <summary>
        /// Gets or sets the configuration for <see cref="IManagementServer"/>
        /// </summary>
        public ManagementConfiguration Management { get; set; }
    }
}