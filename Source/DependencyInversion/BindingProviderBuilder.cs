// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="IBindingProviderBuilder"/>.
    /// </summary>
    public class BindingProviderBuilder : IBindingProviderBuilder
    {
        readonly List<BindingBuilder> _bindings = new();

        /// <inheritdoc/>
        public IBindingBuilder<T> Bind<T>()
        {
            var binding = new Binding(typeof(T), new Strategies.Null(), new Scopes.Transient());
            var bindingBuilder = new BindingBuilder<T>(binding);
            _bindings.Add(bindingBuilder);
            return bindingBuilder;
        }

        /// <inheritdoc/>
        public IBindingBuilder Bind(Type type)
        {
            var binding = new Binding(type, new Strategies.Null(), new Scopes.Transient());
            var bindingBuilder = new BindingBuilder(binding);
            _bindings.Add(bindingBuilder);
            return bindingBuilder;
        }

        /// <inheritdoc/>
        public IBindingCollection Build()
            => new BindingCollection(_bindings.Select(_ => _.Build()));
    }
}