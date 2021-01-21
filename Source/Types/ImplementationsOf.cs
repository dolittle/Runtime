// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Dolittle.Runtime.Types
{
    /// <summary>
    /// Represents an implementation of <see cref="IImplementationsOf{T}"/>.
    /// </summary>
    /// <typeparam name="T">Base type to discover for - must be an abstract class or an interface.</typeparam>
    public class ImplementationsOf<T> : IImplementationsOf<T>
        where T : class
    {
        readonly IEnumerable<Type> _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementationsOf{T}"/> class.
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> to use for finding types.</param>
        public ImplementationsOf(ITypeFinder typeFinder)
        {
            _types = typeFinder.FindMultiple<T>();
        }

        /// <inheritdoc/>
        public IEnumerator<Type> GetEnumerator()
        {
            return _types.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _types.GetEnumerator();
        }
    }
}