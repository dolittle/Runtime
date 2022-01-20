// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.ResourceTypes.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.MongoDB;

/// <summary>
/// Represents a connection to the MongoDB database.
/// </summary>
[SingletonPerTenant]
public class DatabaseConnection
{
    /// <summary>
    /// Initializes static members of the <see cref="DatabaseConnection"/> class.
    /// </summary>
    static DatabaseConnection()
    {
        RegisterCustomDiscriminators();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseConnection"/> class.
    /// </summary>
    /// <param name="configuration">A <see cref="IConfigurationFor{EventStoreConfiguration}"/> with database connection parameters.</param>
    public DatabaseConnection(IConfigurationFor<ProjectionsConfiguration> configuration)
    {
        var config = configuration.Instance;
        var settings = new MongoClientSettings
        {
            Servers = config.Servers.Select(MongoServerAddress.Parse),
            GuidRepresentation = GuidRepresentation.Standard,
            MaxConnectionPoolSize = config.MaxConnectionPoolSize,
        };

        MongoClient = new MongoClient(settings.Freeze());
        Database = MongoClient.GetDatabase(config.Database);
    }

    /// <summary>
    /// Gets the configured <see cref="IMongoClient"/> for the MongoDB database.
    /// </summary>
    public IMongoClient MongoClient { get; }

    /// <summary>
    /// Gets the configured <see cref="IMongoDatabase"/> for the MongoDB database.
    /// </summary>
    public IMongoDatabase Database { get; }

    /// <summary>
    /// Sets our custom <see cref="IDiscriminatorConvention"/>'s.
    /// </summary>
    /// <remarks>
    /// DiscriminatorConventions need to be registered before everything else is done with MongoDB, otherwise the classes
    /// will get assigned a BsonClassMapSerializer implicitly. We can also only register them once, multiple registrations
    /// result in errors.
    /// https://stackoverflow.com/a/30292486/5806412 .
    /// </remarks>
    static void RegisterCustomDiscriminators()
    {
    }
}