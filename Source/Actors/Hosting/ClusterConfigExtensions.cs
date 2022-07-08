// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Proto;
using Proto.Cluster;
using Proto.OpenTelemetry;

namespace Dolittle.Runtime.Actors.Hosting;

/// <summary>
/// Extension methods for <see cref="ClusterConfig"/>.
/// </summary>
public static class ClusterConfigExtensions
{
    static readonly Expression<Func<ClusterIdentity, TenantId>> _parseIdentity = identity => identity.Identity;

    /// <summary>
    /// Registers all discovered <see cref="ClusterKind"/> on the <see cref="ActorSystem"/> <see cref="Cluster"/>.
    /// </summary>
    /// <param name="config">The <see cref="ClusterConfig"/>.</param>
    /// <param name="provider">The <see cref="IServiceProvider"/>.</param>
    /// <returns>The modified <see cref="ClusterConfig"/>.</returns>
    public static ClusterConfig WithDiscoveredClusterKinds(this ClusterConfig config, IServiceProvider provider)
        => config.WithClusterKinds(
            provider.GetRequiredService<IEnumerable<GrainAndActor>>()
                .Select(_ => new ClusterKind(_.Kind, CreatePropsFor(_, provider)))
                .ToArray());

    static object GetTenantGrainFactory(GrainAndActor grainAndActor, IServiceProvider provider)
    {
        var tenantDelegateType = typeof(Func<,,,>).MakeGenericType(
            typeof(TenantId),
            typeof(IContext),
            typeof(ClusterIdentity),
            grainAndActor.Grain);
        var resolvedTenantDelegate = provider.GetRequiredService(tenantDelegateType);
        var contextParameter = Expression.Parameter(typeof(IContext), "context");
        var clusterIdentityParameter = Expression.Parameter(typeof(ClusterIdentity), "identity");

        return Expression.Lambda(
            Expression.Invoke(
                Expression.Constant(resolvedTenantDelegate),
                Expression.Invoke(_parseIdentity, clusterIdentityParameter),
                contextParameter,
                clusterIdentityParameter
            ),
            contextParameter,
            clusterIdentityParameter
        ).Compile();
    }
    
    static Props CreatePropsFor(GrainAndActor grainAndActor, IServiceProvider provider)
        => Props.FromProducer(() => Activator.CreateInstance(
            grainAndActor.Actor,
            grainAndActor.IsPerTenant
                ? GetTenantGrainFactory(grainAndActor, provider)
                : GetGrainFactory(grainAndActor, provider)) as IActor)
            .WithTracing()
            .WithClusterRequestDeduplication(TimeSpan.FromSeconds(60)
            );

    static object GetGrainFactory(GrainAndActor grainAndActor, IServiceProvider provider)
        => provider.GetRequiredService(typeof(Func<,,>).MakeGenericType(
            typeof(IContext),
            typeof(ClusterIdentity),
            grainAndActor.Grain));
}
