// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.ResourceTypes
{
    /// <summary>
    /// Represents the concept of a resrouce type implementation - an identifier.
    /// </summary>
    public class ResourceTypeImplementation : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to an <see cref="ResourceTypeImplementation"/>.
        /// </summary>
        /// <param name="value">String value representing the <see cref="ResourceTypeImplementation"/>.</param>
        public static implicit operator ResourceTypeImplementation(string value) => new ResourceTypeImplementation { Value = value };
    }
}