// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.DependencyInversion.Autofac
{
    /// <summary>
    /// Exception that gets thrown when no binding exists for a service.
    /// </summary>
    public class UnableToFindBindingForService : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToFindBindingForService"/> class.
        /// </summary>
        /// <param name="service"><see cref="Type">Service</see> missing binding.</param>
        public UnableToFindBindingForService(Type service)
            : base($"Couldn't find a binding for service {service.AssemblyQualifiedName}")
        {
        }
    }
}