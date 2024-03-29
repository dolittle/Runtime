// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proto;
using Proto.Cluster;
using Proto.Remote;
using Proto.Cluster.SingleNode;
using Proto.DependencyInjection;
using Proto.OpenTelemetry;
using Proto.Remote.GrpcNet;


namespace Dolittle.Runtime.Actors.Hosting;

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
            .AddSingleton(provider => new ActorSystem(new ActorSystemConfig
                {
                    ConfigureProps = props => props with { StartDeadline = TimeSpan.Zero },
                    ConfigureSystemProps = (_, props) => props with { StartDeadline = TimeSpan.Zero },
                })
                .WithServiceProvider(provider)
                .WithRemote(GrpcNetRemoteConfig
                    .BindToLocalhost()
                    .WithProtoMessages(provider.GetRequiredService<IProtobufFileDescriptors>().All.ToArray()))
                .WithCluster(ClusterConfig.Setup(
                        "Dolittle",
                        new SingleNodeProvider(),
                        new SingleNodeLookup())
                    .WithDiscoveredClusterKinds(provider)))
            .AddSingleton(provider => provider.GetRequiredService<ActorSystem>().Root.WithTracing())
            .AddSingleton(provider => provider.GetRequiredService<ActorSystem>().Cluster()));
}
