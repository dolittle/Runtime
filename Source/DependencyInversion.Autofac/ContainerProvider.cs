// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;
using Dolittle.Runtime.Assemblies;

namespace Dolittle.Runtime.DependencyInversion.Autofac;

/// <summary>
/// Represents async implementation of <see cref="ICanProvideContainer"/> specific for Autofac.
/// </summary>
public class ContainerProvider : ICanProvideContainer
{
    /// <inheritdoc/>
    public IContainer Provide(IAssemblies assemblies, IBindingCollection bindings)
    {
        var containerBuilder = new ContainerBuilder();
        containerBuilder.AddDolittle(assemblies, bindings);
        var autofacContainer = containerBuilder.Build();
        return new Container(autofacContainer);
    }
}