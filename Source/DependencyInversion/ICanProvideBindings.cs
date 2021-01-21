// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Defines a system that can provide <see cref="Binding">bindings</see>.
    /// </summary>
    public interface ICanProvideBindings
    {
        /// <summary>
        /// Method that gets called to provide bindings.
        /// </summary>
        /// <param name="builder">The <see cref="IBindingBuilder"/> to use for building and providing bindings.</param>
        void Provide(IBindingProviderBuilder builder);
    }
}