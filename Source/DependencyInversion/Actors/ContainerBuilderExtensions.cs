// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Autofac;
using Dolittle.Runtime.DependencyInversion.Scoping;
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
        builder.RegisterTypes(classes.Grain).AsSelf();
        foreach (var grainType in classes.Grain)
        {
            var actorType = grainType.GetCustomAttribute<GrainAttribute>().ActorType;
            var grainFactoryType = typeof(Func<>).MakeGenericType(
                typeof(IContext),
                typeof(ClusterIdentity),
                grainType);
            
            builder
                .Register<Func<IContext, ClusterIdentity, object>>(ctx => (context, id) => ctx.Resolve(grainType, new TypedParameter(typeof(IContext), context), new TypedParameter(typeof(ClusterIdentity), id)))
                .As(grainFactoryType);
            builder.RegisterType(actorType).AsSelf();
        }
    }
}
