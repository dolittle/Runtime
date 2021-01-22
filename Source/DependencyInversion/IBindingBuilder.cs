// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Defines a builder of <see cref="Binding"/>.
    /// </summary>
    public interface IBindingBuilder
    {
        /// <summary>
        /// Bind to a type.
        /// </summary>
        /// <typeparam name="T">Type to bind to.</typeparam>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To<T>();

        /// <summary>
        /// Bind to a type.
        /// </summary>
        /// <param name="type">Type to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(Type type);

        /// <summary>
        /// Bind to a constant.
        /// </summary>
        /// <param name="constant">Constant to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(object constant);

        /// <summary>
        /// Bind to a callback.
        /// </summary>
        /// <param name="callback">Callback to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(Func<object> callback);

        /// <summary>
        /// Bind to a callback with <see cref="BindingContext"/> passed in.
        /// </summary>
        /// <param name="callback">Callback to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(Func<BindingContext, object> callback);

        /// <summary>
        /// Bind to a callback for defining the type to bind to.
        /// </summary>
        /// <param name="callback">Callback to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(Func<Type> callback);

        /// <summary>
        /// Bind to a callback for defining the type to bind to with <see cref="BindingContext"/> passed in.
        /// </summary>
        /// <param name="callback">Callback to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(Func<BindingContext, Type> callback);

        /// <summary>
        /// Builds the Binding.
        /// </summary>
        /// <returns>The resulting <see cref="Binding"/>.</returns>
        Binding Build();
    }

    /// <summary>
    /// Defines a typed builder of <see cref="Binding"/>.
    /// </summary>
    /// <typeparam name="T">Type the builder is for.</typeparam>
    public interface IBindingBuilder<T> : IBindingBuilder
    {
        /// <summary>
        /// Bind to a type.
        /// </summary>
        /// <typeparam name="TTarget">Target type to bind to.</typeparam>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        new IBindingScopeBuilder To<TTarget>()
            where TTarget : T;

        /// <summary>
        /// Bind to a constant.
        /// </summary>
        /// <param name="constant">Constant to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(T constant);

        /// <summary>
        /// Bind to a callback that returns an instance of the type or derivative we're building for.
        /// </summary>
        /// <param name="callback">Callback to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(Func<T> callback);

        /// <summary>
        /// Bind to a callback that returns an instance of the type or derivative we're building for
        /// with <see cref="BindingContext"/> passed into it.
        /// </summary>
        /// <param name="callback">Callback to bind to.</param>
        /// <returns><see cref="IBindingScopeBuilder"/> for building scope.</returns>
        IBindingScopeBuilder To(Func<BindingContext, T> callback);
    }
}