// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store
{
    /// <summary>
    /// Represents the projection key used to identify a specific state from a projection.
    /// </summary>
    /// <param name="Value">The key as a string.</param>
    /// <typeparam name="string">The type of the concept.</typeparam>
    public record ProjectionKey(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicit operator from string.
        /// </summary>
        /// <param name="key">The projection key.</param>
        public static implicit operator ProjectionKey(string key) => new(key);
    }
}
