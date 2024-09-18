// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Microsoft.Extensions.Options;
using Testcontainers.MongoDb;
using Xunit;

namespace Events.Store.MongoDB.Tests;

public class MongoDatabaseFixture: IAsyncLifetime
{
    IDatabaseConnection? _connection;
    
    public IDatabaseConnection Connection => _connection ?? throw new Exception("Connection not initialized");
    
    MongoDbContainer MongoDbContainer { get; }

    public MongoDatabaseFixture()
    {
        MongoDbContainer = new MongoDbBuilder()
            .WithReplicaSet("rs0")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await MongoDbContainer.StartAsync();
        var connectionString = MongoDbContainer.GetConnectionString();
        var eventStoreConfiguration = new EventStoreConfiguration
        {
            ConnectionString = connectionString,
            Database = "EventStore"
        };
        _connection = new DatabaseConnection(Options.Create<EventStoreConfiguration>(eventStoreConfiguration), new BackwardsCompatibility());
    }

    public async Task DisposeAsync()
    {
        _connection = null;
        await MongoDbContainer.DisposeAsync();
    }
}