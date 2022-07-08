// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;
using Dolittle.Runtime.DependencyInversion.Types;

namespace Dolittle.Runtime.DependencyInversion;

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
    public static void RegisterClassesByLifecycle(this ContainerBuilder builder, ClassesByLifecycle classes)
    {
        builder.RegisterTypes(classes.SingletonClasses).AsImplementedInterfaces().SingleInstance();
        builder.RegisterTypes(classes.ScopedClasses).AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterTypes(classes.TransientClasses).AsImplementedInterfaces();
    }
}
