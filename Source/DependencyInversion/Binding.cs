// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Represents a binding definition for any IOC container to hook up.
    /// </summary>
    public record Binding(Type Service, IActivationStrategy Strategy, IScope Scope)
    {
        /// <summary>
        /// Create a <see cref="Binding"/> for a specific service <see cref="Type"/>.
        /// </summary>
        /// <param name="service"><see cref="Type"/> of paramref name="service" to create <see cref="Binding"/> for.</param>
        /// <returns><see cref="Binding"/>.</returns>
        public static Binding For(Type service) => new(service, new Strategies.Null(), new Scopes.Transient());
    }
}