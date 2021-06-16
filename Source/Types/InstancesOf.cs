// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;

namespace Dolittle.Runtime.Types
{
    /// <summary>
    /// Represents an implementation of <see cref="IInstancesOf{T}"/>.
    /// </summary>
    /// <typeparam name="T">Base type to discover for - must be an abstract class or an interface.</typeparam>
    public class InstancesOf<T> : IInstancesOf<T>
        where T : class
    {
        readonly IEnumerable<Type> _types;
        readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstancesOf{T}"/> class.
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> used for finding types.</param>
        /// <param name="container"><see cref="IContainer"/> used for managing instances of the types when needed.</param>
        public InstancesOf(ITypeFinder typeFinder, IContainer container)
        {
            _types = typeFinder.FindMultiple<T>();
            _container = container;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var type in _types) yield return _container.Get(type) as T;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var type in _types) yield return _container.Get(type);
        }
    }
}
