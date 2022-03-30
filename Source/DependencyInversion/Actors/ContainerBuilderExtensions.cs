// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Dolittle.Runtime.DependencyInversion.Types;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.DependencyInversion.Actors;

/// <summary>
/// Extension methods for <see cref="ContainerBuilder"/>.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Registers a set of discovered <see cref="ClassesByLifecycle"/> in the <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="builder">The container builder to register types in.</param>
    /// <param name="classes">The classes grouped by lifecycle to register.</param>
    public static void RegisterActorsByActorType(this ContainerBuilder builder, ClassesByActorType classes)
    {
        builder.RegisterTypes(classes.Actor).AsSelf();
        foreach (var (grain, actor) in classes.Grain)
        {
            builder.RegisterTypes(actor, grain).AsSelf();
        }
    }
    /// <summary>
    /// Adds registrations for dependencies of type <see cref="Func{TResult}"/> where the first parameter is a <see cref="IContext"/>
    /// and second is <see cref="ClusterIdentity"/>, by delegating the resolving to the per-tenant containers.
    /// </summary>
    /// <param name="builder">The container builder to add the registrations to.</param>
    /// <returns>The container builder for continuation.</returns>
    public static ContainerBuilder AddClusterKindFactories(this ContainerBuilder builder)
    {
        builder.RegisterSource<GeneratedClusterKindFactoryRegistrationSource>();
        return builder;
    }
}
