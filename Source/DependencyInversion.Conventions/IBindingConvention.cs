// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Conventions
{
    /// <summary>
    /// Defines the basic functionality for a convention that can be applied
    /// to bindings for a <see cref="IContainer"/>.
    /// </summary>
    /// <remarks>
    /// Types inheriting from this interface will be automatically registered.
    /// An application can implement any number of these conventions, in addition to
    /// the <see cref="DefaultConvention"/> supplied by Dolittle.
    /// </remarks>
    public interface IBindingConvention
    {
        /// <summary>
        /// Checks wether or not a given <see cref="Type">Service</see> can be resolved by the convention.
        /// </summary>
        /// <param name="service">Service that needs to be resolved.</param>
        /// <returns>True if it can resolve it, false if not.</returns>
        bool CanResolve(Type service);

        /// <summary>
        /// Resolve a <see cref="Type">Service</see>.
        /// </summary>
        /// <param name="service">Service that needs to be resolved.</param>
        /// <param name="bindingBuilder"><see cref="IBindingBuilder"/> to build.</param>
        void Resolve(Type service, IBindingBuilder bindingBuilder);
    }
}