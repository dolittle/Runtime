// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion
{
    /// <summary>
    /// Represents the context for a binding, typically used in callbacks that resolve instance or type.
    /// </summary>
    public class BindingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingContext"/> class.
        /// </summary>
        /// <param name="service"><see cref="Type">Service type</see> the context is for.</param>
        public BindingContext(Type service)
        {
            Service = service;
        }

        /// <summary>
        /// Gets the service being asked for.
        /// </summary>
        public Type Service { get; }
    }
}