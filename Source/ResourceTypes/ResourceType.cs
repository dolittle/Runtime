// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.ResourceTypes
{
    /// <summary>
    /// Represents the type of a resource in the system.
    /// </summary>
    public record ResourceType(string Value)
    {
        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="ResourceType"/>.
        /// </summary>
        /// <param name="value">String value to convert from.</param>
        public static implicit operator ResourceType(string value) => new(value);
    }
}