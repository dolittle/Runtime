// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Represents a <see cref="IBindingBuilder{T}"/> for a specific type.
/// </summary>
/// <typeparam name="T">Type the builder is for.</typeparam>
public class BindingBuilder<T> : BindingBuilder, IBindingBuilder<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindingBuilder{T}"/> class.
    /// </summary>
    /// <param name="binding"><see cref="Binding"/> for the builder.</param>
    public BindingBuilder(Binding binding)
        : base(binding)
    {
    }

    /// <inheritdoc/>
    IBindingScopeBuilder IBindingBuilder<T>.To<TTarget>()
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.Type(typeof(TTarget)),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(T constant)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.Constant(constant),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(Func<T> callback)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.Callback<T>(callback),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(Func<BindingContext, T> callback)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.CallbackWithBindingContext<T>(callback),
            Binding.Scope));
}