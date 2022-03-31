// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dolittle.Runtime.DependencyInversion.Actors;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.Actors.Hosting;

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
        foreach (var grainAndActor in provider.GetRequiredService<IEnumerable<GrainAndActor>>())
        {
            var delegateType = typeof(Func<,,>).MakeGenericType(
                typeof(IContext),
                typeof(ClusterIdentity),
                grainAndActor.Grain);
            
            // var tenantDelegateType = typeof(Func<,,,>).MakeGenericType(
            //     typeof(TenantId),
            //     typeof(IContext),
            //     typeof(ClusterIdentity),
            //     grainAndActor.Grain);
            //
            // // Func<ClusterIdentity, TenantId> parser = identity => identity.Identity;
            //
            // dynamic resolvedTenantDelegate = provider.GetRequiredService(tenantDelegateType) as Delegate;
            //
            // var contextParameter = Expression.Parameter(typeof(IContext), "context");
            // var clusterIdentityParameter = Expression.Parameter(typeof(ClusterIdentity), "identity");
            //
            // var constructedDelegate = Expression.Lambda(
            //         Expression.Invoke(
            //             resolvedTenantDelegate,
            //             Expression.Call(
            //                 null,
            //                 typeof(ClusterConfigExtensions).GetMethod("Parse", BindingFlags.Static | BindingFlags.NonPublic),
            //                 clusterIdentityParameter),
            //             contextParameter,
            //             clusterIdentityParameter
            //         ),
            //         contextParameter,
            //         clusterIdentityParameter
            //     ).Compile();
            //

            var resolvedDelegate = provider.GetRequiredService(delegateType);

            config = config.WithClusterKind(
                grainAndActor.Kind,
                Props.FromProducer(() => Activator.CreateInstance(
                    grainAndActor.Actor,
                    resolvedDelegate) as IActor));
                    // grainAndActor.IsPerTenant ? constructedDelegate : resolvedDelegate) as IActor));
        }

        return config;
        
        // return config
        //     .WithClusterKinds(globalActors.Grain.Concat(perTenantActors.Grain)
        //         .Select(grainAndActor => new ClusterKind(
        //             grainAndActor.Kind,
        //             Props.FromProducer(() => Activator.CreateInstance(
        //                 grainAndActor.Actor,
        //                 provider.GetRequiredService(Expression.GetDelegateType(typeof(IContext), typeof(ClusterIdentity), grainAndActor.Grain))) as IActor))).ToArray());
    }

    static TenantId Parse(ClusterIdentity id) => id.Identity;
}
