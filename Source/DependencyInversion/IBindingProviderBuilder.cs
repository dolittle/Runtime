// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Defines a system for building <see cref="Binding">bindings</see> that can be provided.
/// </summary>
public interface IBindingProviderBuilder
{
    /// <summary>
    /// Bind a specific type by its generic parameter.
    /// </summary>
    /// <typeparam name="T">Type of service to bind.</typeparam>
    /// <returns><see cref="IBindingBuilder{T}"/> for building the actual binding.</returns>
    IBindingBuilder<T> Bind<T>();

    /// <summary>
    /// Bind a specific type.
    /// </summary>
    /// <param name="type">Type of service to bind.</param>
    /// <returns><see cref="IBindingBuilder"/> for building the actual binding.</returns>
    IBindingBuilder Bind(Type type);

    /// <summary>
    /// Builds the <see cref="IBindingCollection"/> for the provider.
    /// </summary>
    /// <returns><see cref="IBindingCollection"/> with all bindings built.</returns>
    IBindingCollection Build();
}