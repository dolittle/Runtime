/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Configuration;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents the configuration of the server
    /// </summary>
    [Name("server")]
    public class Configuration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Configuration"/>
        /// </summary>
        public Configuration(InteractionConfiguration interaction, ManagementConfiguration management)
        {
            Interaction = interaction;
            Management = management;
        }

        /// <summary>
        /// Gets or sets the configuration for <see cref="IInteractionServer"/>
        /// </summary>
        public InteractionConfiguration Interaction { get; }

        /// <summary>
        /// Gets or sets the configuration for <see cref="IManagementServer"/>
        /// </summary>
        public ManagementConfiguration Management { get; }
    }
}