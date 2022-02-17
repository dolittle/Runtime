// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Dolittle.Runtime.Configuration.ConfigurationObjects.Resources;
using Dolittle.Runtime.DependencyInversion;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Dolittle.Runtime.Resources.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IKnowTheConnectionString"/>.
/// </summary>
[SingletonPerTenant]
public class ConnectionStringFromResourceConfiguration : IKnowTheConnectionString
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringFromResourceConfiguration"/> class. 
    /// </summary>
    /// <param name="configuration">The <see cref="IOptions{T}"/> of type <see cref="ResourceConfiguration"/> to use.</param>
    public ConnectionStringFromResourceConfiguration(IOptions<ResourceConfiguration> configuration)
    {
        ConnectionString = BuildConnectionString(configuration.Value);
    }

    /// <inheritdoc />
    public MongoUrl ConnectionString { get; }

    static MongoUrl BuildConnectionString(ResourceConfiguration configuration)
        => new MongoUrlBuilder(configuration.Host)
        {
            UseTls = configuration.UseSSL,
            DatabaseName = configuration.Database
        }.ToMongoUrl();
}
