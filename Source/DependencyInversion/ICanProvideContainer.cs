// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Assemblies;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a system that provide a <see cref="IContainer"/> implementation.
/// </summary>
public interface ICanProvideContainer
{
    /// <summary>
    /// Provide the container prebuilt with the given bindings.
    /// </summary>
    /// <param name="assemblies"><see cref="IAssemblies"/> for the application.</param>
    /// <param name="bindings"><see cref="IBindingCollection">Bindings</see> provided.</param>
    /// <returns>A <see cref="IContainer"/> instance.</returns>
    IContainer Provide(IAssemblies assemblies, IBindingCollection bindings);
}