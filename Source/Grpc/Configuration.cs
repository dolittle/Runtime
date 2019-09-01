/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Configuration;
using Dolittle.Grpc;

namespace Dolittle.Runtime.Grpc
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
        public Configuration(HostConfiguration interaction, HostConfiguration management)
        {
            Interaction = interaction;
            Management = management;
        }

        /// <summary>
        /// Gets or sets the configuration for interaction <see cref="IHost"/>
        /// </summary>
        public HostConfiguration Interaction {  get; }

        /// <summary>
        /// Gets or sets the configuration for management <see cref="IHost"/>
        /// </summary>
        public HostConfiguration Management {  get; }
    }
}