// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.ResourceTypes.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB;

/// <summary>
/// Represents a connection to the MongoDB database.
/// </summary>
[SingletonPerTenant]
public class DatabaseConnection
{

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseConnection"/> class.
    /// </summary>
    /// <param name="configuration">A <see cref="IConfigurationFor{EmbeddingsConfiguration}"/> with database connection parameters.</param>
    public DatabaseConnection(IConfigurationFor<EmbeddingsConfiguration> configuration)
    {
        var config = configuration.Instance;
        var settings = new MongoClientSettings
        {
            Servers = config.Servers.Select(_ => MongoServerAddress.Parse(_)),
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
}