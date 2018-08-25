/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.IO;
using Dolittle.Logging;
using Dolittle.Serialization.Json;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents an implementation of <see cref="IConfigurationManager"/>
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        const string _configurationFile = ".dolittle/dolittle-server.json";

        readonly ILogger _logger;
        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationManager"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use for JSON serialization</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public ConfigurationManager(
            ISerializer serializer,
            ILogger logger)
        {
            _logger = logger;
            _serializer = serializer;
            Current = LoadConfig();
            if( Current == null ) Current = new Configuration();

            _logger.Information($"Interaction server will be exposed on TCP with port {Current.Interaction.Port}");
            _logger.Information($"Interaction server will be exposed on Unix socket {Current.Interaction.UnixSocket}");
            _logger.Information($"Management server will be exposed on TCP with port {Current.Management.Port}");
            _logger.Information($"Management server will be exposed on Unix socket {Current.Management.UnixSocket}");
        }

        /// <inheritdoc/>
        public Configuration Current { get; }

        Configuration LoadConfig()
        {
            if( !File.Exists(_configurationFile)) return null;
            var json = File.ReadAllText(_configurationFile);
            return _serializer.FromJson<Configuration>(json);
        }
    }
}