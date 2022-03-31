// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.DependencyInversion.Actors;

/// <summary>
/// Extension methods for <see cref="ContainerBuilder"/>.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Adds registrations for dependencies of type <see cref="Func{TResult}"/> where the first parameter is a <see cref="IContext"/>
    /// and second is <see cref="ClusterIdentity"/>, by delegating the resolving to the per-tenant containers.
    /// </summary>
    /// <param name="builder">The container builder to add the registrations to.</param>
    /// <returns>The container builder for continuation.</returns>
    public static ContainerBuilder AddClusterKindFactories(this ContainerBuilder builder)
    {
        //builder.RegisterSource<GeneratedClusterKindFactoryRegistrationSource>();
        return builder;
    }
}
