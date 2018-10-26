/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.IO;
using Dolittle.Serialization.Json;
using Newtonsoft.Json;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="ITenantsConfigurationManager"/>
    /// </summary>
    public class TenantsConfigurationManager : ITenantsConfigurationManager
    {
        const string _configurationFile = ".dolittle/tenants.json";

        readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of <see cref="TenantsConfigurationManager"/>
        /// </summary>
        /// <param name="serializer"><see cref="ISerializer"/> to use for dealing with configuration as JSON</param>
        public TenantsConfigurationManager(ISerializer serializer)
        {
            _serializer = serializer;
            Current = LoadConfig();
        }

        /// <inheritdoc/>
        public TenantsConfiguration Current { get; }

        TenantsConfiguration LoadConfig()
        {
            if( !File.Exists(_configurationFile)) return new TenantsConfiguration();

            var json = File.ReadAllText(_configurationFile);
            var config = _serializer.FromJson<TenantsConfiguration>(json);
            
            return config;
        }
    }

}