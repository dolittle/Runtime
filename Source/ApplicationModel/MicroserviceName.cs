// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.ApplicationModel
{
    /// <summary>
    /// Represents the name of a <see cref="Microservice"/>.
    /// </summary>
    public record MicroserviceName(string Value)
    {
        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to a <see cref="MicroserviceName"/>.
        /// </summary>
        /// <param name="microserviceName">Name of the <see cref="Microservice"/>.</param>
        public static implicit operator MicroserviceName(string microserviceName) => new (microserviceName);
    }
}
