// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="ServiceType"/> is unknown.
    /// </summary>
    public class UnknownServiceType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownServiceType"/> class.
        /// </summary>
        /// <param name="type">Unknown <see cref="ServiceType"/>.</param>
        public UnknownServiceType(ServiceType type)
            : base($"Unknown service type '{type}'")
        {
        }
    }
}