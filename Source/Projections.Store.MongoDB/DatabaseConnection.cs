﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IDatabaseConnection"/>.
/// </summary>
[Singleton, PerTenant]
public class DatabaseConnection : IDatabaseConnection
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
    public DatabaseConnection(IOptions<ProjectionsConfiguration> configuration)
    {
        var config = configuration.Value;
        MongoClient = new MongoClient(GetMongoClientSettings(config));
        Database = MongoClient.GetDatabase(config.Database);
    }

    static MongoClientSettings GetMongoClientSettings(ProjectionsConfiguration config)
    {
        if(!string.IsNullOrWhiteSpace(config.ConnectionString))
        {
            // CS style settings
            var settings = MongoClientSettings.FromConnectionString(config.ConnectionString);
            settings.GuidRepresentation = GuidRepresentation.Standard;
            settings.MaxConnectionPoolSize = config.MaxConnectionPoolSize;
            settings.ClusterConfigurator = cb => cb.AddTelemetry();
            return settings.Freeze();
        }
        else
        {
            // Legacy settings
            var settings = new MongoClientSettings
            {
                Servers = config.Servers.Select(MongoServerAddress.Parse),
                GuidRepresentation = GuidRepresentation.Standard,
                MaxConnectionPoolSize = config.MaxConnectionPoolSize,
                ClusterConfigurator = cb => cb.AddTelemetry()
            };
            return settings.Freeze();
        }
    }

    
    /// <inheritdoc />
    public IMongoClient MongoClient { get; }

    /// <inheritdoc />
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
