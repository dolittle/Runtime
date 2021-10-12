// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Serialization.Json;
namespace Dolittle.Runtime.CLI.Configurations
{
    /// <summary>
    /// Represents an implementation of <see cref="IConfigurations" />.
    /// </summary>
    public class Configurations : IConfigurations
    {
        readonly ISerializer _serializer;
        readonly RuntimeConfigurationDirectoryPath _configurationDirectoryPath;

        /// <summary>
        /// Initialises an instance of the <see cref="Configurations"/> class.
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> for serializing json.</param>
        /// <param name="configurationDirectoryPath">The <see cref="RuntimeConfigurationDirectoryPath"/>.</param>
        public Configurations(ISerializer serializer, RuntimeConfigurationDirectoryPath configurationDirectoryPath)
        {
            _serializer = serializer;
            _configurationDirectoryPath = Path.Join(configurationDirectoryPath, ".dolittle");
        }

        /// <inheritdoc/>
        public Try<TConfigurationObject> TryGet<TConfigurationObject>(RuntimeConfigurationName configurationName)
        {
            try
            {
                var filepath = Path.Join(_configurationDirectoryPath, configurationName);
                var json = File.ReadAllText(filepath);
                return _serializer.FromJson<TConfigurationObject>(json);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}