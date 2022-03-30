// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.Actors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Testing;
using Proto.DependencyInjection;
using Proto.Remote.GrpcNet;

namespace Dolittle.Runtime.Server.Actors;

/// <summary>
/// Extensions for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Configures the <see cref="ActorSystem"/> service in the <see cref="ServiceCollection"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/>.</param>
    /// <returns>The builder for continuation.</returns>
    public static IHostBuilder AddActorSystem(this IHostBuilder builder)
        => builder.ConfigureServices(_ => _
            .AddSingleton(provider => new ActorSystem(ActorSystemConfig.Setup())
                .WithServiceProvider(provider)
                .WithRemote(GrpcNetRemoteConfig.BindToLocalhost())
                .WithCluster(ClusterConfig.Setup(
                        "Dolittle",
                        new TestProvider(new TestProviderOptions(), new InMemAgent()),
                        new PartitionIdentityLookup())
                    .WithDiscoveredClusterKinds(provider))));
    
}