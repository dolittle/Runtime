// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace Dolittle.Runtime.Types.Testing
{
    /// <summary>
    /// Represents an implementation of <see cref="IInstancesOf{T}"/> for static purposes.
    /// </summary>
    /// <typeparam name="T">Base type to discover for - must be an abstract class or an interface.</typeparam>
    public class StaticInstancesOf<T> : IInstancesOf<T>
        where T : class
    {
        readonly List<T> _instances;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticInstancesOf{T}"/> class.
        /// </summary>
        /// <param name="instances">The collection of well known instances.</param>
        public StaticInstancesOf(params T[] instances)
        {
            _instances = new List<T>(instances);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticInstancesOf{T}"/> class.
        /// </summary>
        /// <param name="instances">The collection of well known instances.</param>
        public StaticInstancesOf(IEnumerable<T> instances)
        {
            _instances = new List<T>(instances);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return _instances.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _instances.GetEnumerator();
        }
    }
}