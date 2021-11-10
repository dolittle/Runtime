// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.ResourceTypes.Configuration;
using MongoDB.Driver;

namespace Dolittle.Runtime.Resources.MongoDB
{
    /// <summary>
    /// Represents an implementation of <see cref="IResource"/>.
    /// </summary>
    [SingletonPerTenant]
    public class Resource : IResource
    {
        readonly IConfigurationFor<Configuration> _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class. 
        /// </summary>
        /// <param name="configuration">The <see cref="IConfigurationFor{T}"/> <see cref="Configuration"/>.</param>
        public Resource(IConfigurationFor<Configuration> configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public ConnectionDetails GetConnectionDetails()
        {
            var config = _configuration.Instance;
            var builder = new MongoUrlBuilder(config.Host);
            builder.UseTls = config.UseSSL;
            builder.DatabaseName = config.Database;
            return new ConnectionDetails(builder.ToMongoUrl(), builder.DatabaseName);
        }
    }
}
