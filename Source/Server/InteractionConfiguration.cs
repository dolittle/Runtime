/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents the configuration for the <see cref="IInteractionServer"/>
    /// </summary>
    public class InteractionConfiguration
    {
        /// <summary>
        /// Gets or sets whether or not the interaction server is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The port to use for exposing the <see cref="IInteractionServer"/> on
        /// </summary>
        public int Port { get; set; } = 50051;

        /// <summary>
        /// The unix socket to use for exposing the <see cref="IInteractionServer"/> on
        /// </summary>
        public string UnixSocket { get; set; } = "/var/run/dolittle.interaction.sock";
    }
}