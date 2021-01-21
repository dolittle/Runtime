// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Reflection;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Represents an implementation of <see cref="IBindingScopeBuilder"/>.
    /// </summary>
    public class BindingScopeBuilder : IBindingScopeBuilder
    {
        Binding _binding;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingScopeBuilder"/> class.
        /// </summary>
        /// <param name="binding"><see cref="Binding"/> to build for.</param>
        public BindingScopeBuilder(Binding binding)
        {
            _binding = binding;
        }

        /// <inheritdoc/>
        public void Singleton()
        {
            _binding = new Binding(
                _binding.Service,
                _binding.Strategy,
                new Scopes.Singleton());
        }

        /// <inheritdoc/>
        public void SingletonPerTenant()
        {
            _binding = new Binding(
                _binding.Service,
                _binding.Strategy,
                new Scopes.SingletonPerTenant());
        }

        /// <inheritdoc/>
        public Binding Build()
        {
            if (!(_binding.Scope is Scopes.Singleton) && _binding.Strategy.GetTargetType().HasAttribute<SingletonAttribute>())
                Singleton();

            if (!(_binding.Scope is Scopes.SingletonPerTenant) && _binding.Strategy.GetTargetType().HasAttribute<SingletonPerTenantAttribute>())
                SingletonPerTenant();

            return _binding;
        }
    }
}