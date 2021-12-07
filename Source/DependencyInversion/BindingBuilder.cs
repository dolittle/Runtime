// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion;

/// <summary>
/// Represents an implementation of <see cref="IBindingBuilder"/>.
/// </summary>
public class BindingBuilder : IBindingBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindingBuilder"/> class.
    /// </summary>
    /// <param name="binding"><see cref="DependencyInversion.Binding"/> to build from.</param>
    public BindingBuilder(Binding binding)
    {
        Binding = binding;
        ScopeBuilder = new BindingScopeBuilder(binding);
    }

    /// <summary>
    /// Gets or sets the binding we're currently building.
    /// </summary>
    protected Binding Binding { get; set; }

    /// <summary>
    /// Gets or sets the scope builder we're currently building with.
    /// </summary>
    protected IBindingScopeBuilder ScopeBuilder { get; set; }

    /// <inheritdoc/>
    public IBindingScopeBuilder To<T>()
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.Type(typeof(T)),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(Type type)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.Type(type),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(object constant)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.Constant(constant),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(Func<object> callback)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.Callback(callback),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(Func<Type> callback)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.TypeCallback(callback),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(Func<BindingContext, object> callback)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.CallbackWithBindingContext(callback),
            Binding.Scope));

    /// <inheritdoc/>
    public IBindingScopeBuilder To(Func<BindingContext, Type> callback)
        => SetBinding(new Binding(
            Binding.Service,
            new Strategies.TypeCallbackWithBindingContext(callback),
            Binding.Scope));

    /// <inheritdoc/>
    public Binding Build()
    {
        Binding = ScopeBuilder.Build();
        return Binding;
    }

    protected IBindingScopeBuilder SetBinding(Binding binding)
    {
        Binding = binding;
        ScopeBuilder = new BindingScopeBuilder(Binding);
        return ScopeBuilder;
    }
}