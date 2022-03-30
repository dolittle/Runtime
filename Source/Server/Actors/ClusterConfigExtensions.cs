// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using Dolittle.Runtime.DependencyInversion.Actors;
using Microsoft.Extensions.DependencyInjection;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.Server.Actors;

/// <summary>
/// Extension methods for <see cref="ClusterConfig"/>.
/// </summary>
public static class ClusterConfigExtensions
{
    /// <summary>
    /// Registers all discovered <see cref="ClusterKind"/> on the <see cref="ActorSystem"/> <see cref="Cluster"/>.
    /// </summary>
    /// <param name="config">The <see cref="ClusterConfig"/>.</param>
    /// <param name="provider">The <see cref="IServiceProvider"/>.</param>
    /// <returns>The modified <see cref="ClusterConfig"/>.</returns>
    public static ClusterConfig WithDiscoveredClusterKinds(this ClusterConfig config, IServiceProvider provider)
    {
        var (globalActors, perTenantActors) = provider.GetRequiredService<ClassesByScopeAndActorType>();
        return config
            .WithClusterKinds(globalActors.Grain.Concat(perTenantActors.Grain)
                .Select(grainAndActor => new ClusterKind(
                    grainAndActor.Kind,
                    Props.FromProducer(() => Activator.CreateInstance(
                        grainAndActor.Actor,
                        provider.GetRequiredService(Expression.GetDelegateType(typeof(IContext), typeof(ClusterIdentity), grainAndActor.Grain))) as IActor))).ToArray());
        // .WithClusterKinds(
        //     globalActors.Grain
        //         .Select(grainAndActor => new ClusterKind(
        //             grainAndActor.Kind,
        //             Props.FromProducer(() => provider.GetRequiredService(grainAndActor.Actor) as IActor)))
        //         .ToArray())
        // .WithClusterKinds(
        //     perTenantActors.Grain
        //         .Select(grainAndActor => new ClusterKind(
        //             grainAndActor.Kind,
        //             Props.FromProducer(() => Activator.CreateInstance(
        //                 grainAndActor.Actor,
        //                 provider.GetRequiredService(Expression.GetDelegateType(typeof(IContext), typeof(ClusterIdentity), grainAndActor.Grain))) as IActor)))
        //         .ToArray());
    }
}
