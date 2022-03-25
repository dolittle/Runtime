// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IDatabaseConnection"/>.
/// </summary>
[Singleton, PerTenant]
public class DatabaseConnection : IDatabaseConnection
{

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseConnection"/> class.
    /// </summary>
    /// <param name="configuration">A <see cref="IOptions{TOptions}"/> with database connection parameters.</param>
    public DatabaseConnection(IOptions<EmbeddingsConfiguration> configuration)
    {
        var config = configuration.Value;
        var settings = new MongoClientSettings
        {
            Servers = config.Servers.Select(_ => MongoServerAddress.Parse(_)),
            GuidRepresentation = GuidRepresentation.Standard,
            MaxConnectionPoolSize = config.MaxConnectionPoolSize,
        };

        MongoClient = new MongoClient(settings.Freeze());
        Database = MongoClient.GetDatabase(config.Database);
    }

    /// <inheritdoc />
    public IMongoClient MongoClient { get; }

    /// <inheritdoc />
    public IMongoDatabase Database { get; }
}
