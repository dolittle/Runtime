// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Collections;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="IBindingCollection"/>.
    /// </summary>
    public class BindingCollection : IBindingCollection
    {
        readonly List<Binding> _bindings = new List<Binding>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingCollection"/> class.
        /// </summary>
        /// <param name="bindingCollections">Params of <see cref="IEnumerable{T}"/> of <see cref="Binding"/>.</param>
        public BindingCollection(params IEnumerable<Binding>[] bindingCollections)
        {
            bindingCollections.ForEach(_ =>
            {
                var newBindings = _.Where(binding => !HasBindingFor(binding.Service));
                _bindings.AddRange(newBindings);
            });
        }

        /// <inheritdoc/>
        public bool HasBindingFor<T>()
        {
            return _bindings.Any(binding => binding.Service.Equals(typeof(T)));
        }

        /// <inheritdoc/>
        public bool HasBindingFor(Type type)
        {
            return _bindings.Any(binding => binding.Service.Equals(type));
        }

        /// <inheritdoc/>
        public IEnumerator<Binding> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }
    }
}