// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Dolittle.Booting;
using Dolittle.Configuration;
using Dolittle.Services;
using Newtonsoft.Json;

#pragma warning disable SA1402

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents a bridge to avoid having breaking changes on configuration now that we a have a new approach
    /// to services hosted and endpoints. This can be removed from version 4.
    /// </summary>
    [Obsolete("Remove from 4.0.0")]
    public class LegacyConfigurationSupport : ICanRunBeforeBootStage<NoSettings>
    {
        /// <inheritdoc/>
        public BootStage BootStage => BootStage.BootProcedures;

        /// <inheritdoc/>
        public void Perform(NoSettings settings, IBootStageBuilder builder)
        {
            var file = Path.Combine(".dolittle", "server.json");
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                var configuration = JsonConvert.DeserializeObject<ServerConfiguration>(json);
                EndpointConfigurationDefaultProvider.DefaultPublicPort = configuration.Interaction.Port;
            }
        }
    }

    /// <summary>
    /// Represents the configuration of the server.
    /// </summary>
    [Name("server")]
    [Obsolete("Remove from 4.0.0")]
    public class ServerConfiguration
    {
        /// <summary>
        /// Gets or sets the configuration for interaction service endpoint.
        /// </summary>
        public InteractionConfiguration Interaction { get; set; }

        /// <summary>
        /// Gets or sets the configuration for management service endpoint.
        /// </summary>
        public ManagementConfiguration Management { get; set; }
    }

    /// <summary>
    /// Represents the configuration for the interaction service endpoint.
    /// </summary>
    public class InteractionConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the interaction service is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the port to use for exposing the interaction service endpoint on.
        /// </summary>
        public int Port { get; set; } = 50051;

        /// <summary>
        /// Gets or sets the unix socket to use for exposing the interaction service endpoint on.
        /// </summary>
        public string UnixSocket { get; set; } = "/var/run/dolittle.interaction.sock";
    }

    /// <summary>
    /// Represents the configuration for the management service endpoint.
    /// </summary>
    public class ManagementConfiguration
    {
        /// <summary>
        /// Gets or sets whether or not the management service is enabled.
        /// </summary>

        /// <summary>
        /// Gets or sets the port to use for exposing the managemnet service endpoint on.
        /// </summary>
        public int Port { get; set; } = 50052;

        /// <summary>
        /// Gets or sets the unix socket to use for exposing the managemnet service endpoint on.
        /// </summary>
        public string UnixSocket { get; set; } = "/var/run/dolittle.management.sock";
    }
}