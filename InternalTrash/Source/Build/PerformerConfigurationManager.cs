// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.IO;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Serialization.Json;

namespace Dolittle.Build
{
    /// <summary>
    /// Represents an implementation of <see cref="IPerformerConfigurationManager"/>.
    /// </summary>
    [Singleton]
    public class PerformerConfigurationManager : IPerformerConfigurationManager
    {
        readonly IFileSystem _fileSystem;
        readonly ISerializer _serializer;
        readonly IBuildMessages _buildMessages;
        IDictionary<string, object> _configObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformerConfigurationManager"/> class.
        /// </summary>
        /// <param name="buildMessages"><see cref="IBuildMessages"/> for outputting messages.</param>
        /// <param name="fileSystem"><see cref="IFileSystem"/> to use.</param>
        /// <param name="serializer">JSON <see cref="ISerializer"/>.</param>
        public PerformerConfigurationManager(IBuildMessages buildMessages, IFileSystem fileSystem, ISerializer serializer)
        {
            _fileSystem = fileSystem;
            _serializer = serializer;
            _buildMessages = buildMessages;
        }

        /// <inheritdoc/>
        public void Initialize(string jsonFile)
        {
            var json = string.Empty;
            try
            {
                _buildMessages.Information($"Initializing from file '${jsonFile}'");
                json = _fileSystem.ReadAllText(jsonFile);
                _configObjects = _serializer.GetKeyValuesFromJson(json);
            }
            catch (Exception)
            {
                _buildMessages.Error($"Error when initializing '{jsonFile}', content: '{json}'");
                throw;
            }
        }

        /// <inheritdoc/>
        public object GetFor(Type configurationType, string name)
        {
            var configObject = _configObjects[name];
            var json = _serializer.ToJson(configObject);
            return _serializer.FromJson(configurationType, json);
        }
    }
}