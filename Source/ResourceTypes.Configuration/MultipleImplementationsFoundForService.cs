// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.ResourceTypes.Configuration
{
    /// <summary>
    /// Exception that gets thrown when multiple implementations for the same service is discovered.
    /// </summary>
    public class MultipleImplementationsFoundForService : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleImplementationsFoundForService"/> class.
        /// </summary>
        /// <param name="service"><see cref="Type"/> of service with multiple implementations.</param>
        public MultipleImplementationsFoundForService(Type service)
            : base($"Multiple implementations for the service {service.FullName} was found")
        {
        }
    }
}