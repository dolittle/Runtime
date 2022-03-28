// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

public static class HostBuilderExtensions
{
    public static IHostBuilder AddActorSystem(this IHostBuilder builder)
        => builder.ConfigureServices(_ => _
            .AddTransient<EventStoreGrain>()
            .AddSingleton(provider =>
            {
                var actorSystemConfig = ActorSystemConfig.Setup();
                var remoteConfig = GrpcNetRemoteConfig.BindToLocalhost();

                var clusterConfig = ClusterConfig.Setup(
                        clusterName: "Dolittle",
                        clusterProvider: new TestProvider(new TestProviderOptions(), new InMemAgent()),
                        identityLookup: new PartitionIdentityLookup())
                    .WithClusterKind(
                        kind: EventStoreGrainActor.Kind,
                        prop: Props.FromProducer(provider.GetRequiredService<EventStoreGrainActor>));

                return new ActorSystem(actorSystemConfig)
                    .WithServiceProvider(provider)
                    .WithRemote(remoteConfig)
                    .WithCluster(clusterConfig);
            }));
}
