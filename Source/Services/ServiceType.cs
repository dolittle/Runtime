// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an identifier for a service type.
    /// </summary>
    public record ServiceType(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="ServiceType"/>.
        /// </summary>
        /// <param name="type"><see cref="ServiceType"/> as string.</param>
        public static implicit operator ServiceType(string type) => new(type);
    }
}