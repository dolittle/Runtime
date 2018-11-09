/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Serialization.Json;
using Newtonsoft.Json;

namespace Dolittle.Applications.Configuration
{
    /// <inheritdoc/>
    [Singleton]
    public class BoundedContextLoader : IBoundedContextLoader
    {
        readonly ISerializer _serializer;
        readonly ILogger _logger;

        readonly ISerializationOptions _serializationOptions = SerializationOptions.Custom(callback:
            serializer =>
            {
                serializer.ContractResolver = new CamelCaseExceptDictionaryKeyResolver();
                serializer.Formatting = Formatting.Indented;
            }
        );

        BoundedContextConfiguration _instance;

        /// <summary>
        /// Initializes a new instance of <see cref="BoundedContextLoader"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use for working with configuration as JSON</param>
        /// <param name="logger"></param>
        public BoundedContextLoader(ISerializer serializer, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
        }

        /// <inheritdoc/>
        public BoundedContextConfiguration Load(string relativePath)
        {
            if (_instance != null) return _instance;

            var path = GetPath(relativePath);
            if( !File.Exists(path)) throw new MissingBoundedContextConfiguration(path);
            
            var json = File.ReadAllText(path);
            var configuration = _serializer.FromJson<BoundedContextConfiguration>(json, _serializationOptions);
            _instance = configuration;
            return configuration;
        }
        
        string GetPath(string relativePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        }
    }
}