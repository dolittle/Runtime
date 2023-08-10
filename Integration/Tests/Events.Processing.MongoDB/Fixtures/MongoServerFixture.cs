// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Microsoft.Extensions.Options;
using Testcontainers.MongoDb;

namespace Events.Processing.MongoDB.Fixtures;

public class mongo_server_fixture : IAsyncLifetime
{
    public MongoDbContainer container { get; internal set; } = null!;

    public async Task InitializeAsync()
    {
        container = new MongoDbBuilder()
            .Build();
        await container.StartAsync();
        var cs = new Uri(container.GetConnectionString());
        var serverAddress = cs.Host + ":" + cs.Port;
        var authEnabled = TryGetUserNameAndPassword(cs, out var userName, out var password);
        
        var bw = new BackwardsCompatibility(Options.Create(new EventStoreBackwardsCompatibilityConfiguration
        {
            Version = EventStoreBackwardsCompatibleVersion.V6
        }));
        var options = Options.Create(
            new EventStoreConfiguration
            {
                Database = "test",
                UserName = userName,
                Password = password,
                Servers = new[]
                {
                    serverAddress
                }
            });
        database_connection = new DatabaseConnection(options, bw);
    }

    public IDatabaseConnection database_connection { get; internal set; } = null!;

    public Task DisposeAsync()
    {
        return container.DisposeAsync().AsTask();
    }

    static bool TryGetUserNameAndPassword(Uri cs, out string? userName, out string? password)
    {
        userName = null;
        password = null;
        if (cs.UserInfo.Length > 0)
        {
            var parts = cs.UserInfo.Split(':');
            userName = parts[0];
            password = parts[1];
            return true;
        }

        return false;
    }
}