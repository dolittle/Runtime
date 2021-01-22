// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Represents a binding definition for any IOC container to hook up.
    /// </summary>
    public class Binding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="service"><see cref="Type"/> of service.</param>
        /// <param name="strategy"><see cref="IActivationStrategy"/> for the service.</param>
        /// <param name="scope"><see cref="IScope"/> for the service.</param>
        public Binding(Type service, IActivationStrategy strategy, IScope scope)
        {
            Service = service;
            Strategy = strategy;
            Scope = scope;
        }

        /// <summary>
        /// Gets the Service type that the binding is representing.
        /// </summary>
        public Type Service { get; }

        /// <summary>
        /// Gets the <see cref="IActivationStrategy"/> for the <see cref="Binding"/>.
        /// </summary>
        public IActivationStrategy Strategy { get; }

        /// <summary>
        /// Gets the <see cref="IScope"/> for the <see cref="Binding"/>.
        /// </summary>
        public IScope Scope { get; }

        /// <summary>
        /// Create a <see cref="Binding"/> for a specific service <see cref="Type"/>.
        /// </summary>
        /// <param name="service"><see cref="Type"/> of paramref name="service" to create <see cref="Binding"/> for.</param>
        /// <returns><see cref="Binding"/>.</returns>
        public static Binding For(Type service)
        {
            return new Binding(service, new Strategies.Null(), new Scopes.Transient());
        }
    }
}