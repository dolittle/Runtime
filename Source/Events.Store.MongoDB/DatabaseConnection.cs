﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;
using Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;

using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB;

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
    /// <param name="configuration">A <see cref="IOptions{TOptions}"/> with database connection parameters.</param>
    public DatabaseConnection(IOptions<EventStoreConfiguration> configuration)
    {
        var config = configuration.Value;
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
        BsonSerializer.RegisterDiscriminatorConvention(typeof(AbstractStreamProcessorState), new StreamProcessorStateDiscriminatorConvention());
        BsonSerializer.RegisterDiscriminatorConvention(typeof(AbstractFilterDefinition), new FilterDefinitionDiscriminatorConvention());
    }
}
