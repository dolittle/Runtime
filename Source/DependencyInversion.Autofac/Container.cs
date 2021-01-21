// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Autofac;

namespace Dolittle.Runtime.DependencyInversion.Autofac
{
    /// <summary>
    /// Represents an implementation of <see cref="IContainer"/> specific for Autofac.
    /// </summary>
    public class Container : IContainer
    {
        readonly global::Autofac.IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="container"><see cref="global::Autofac.IContainer"/> instance.</param>
        public Container(global::Autofac.IContainer container)
        {
            _container = container;
        }

        /// <inheritdoc/>
        public T Get<T>()
        {
            return _container.Resolve<T>();
        }

        /// <inheritdoc/>
        public object Get(Type type)
        {
            return _container.Resolve(type);
        }
    }
}