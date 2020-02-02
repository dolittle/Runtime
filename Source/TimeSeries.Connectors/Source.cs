// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Represents the concept of an System.
    /// </summary>
    public class Source : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="Source"/>.
        /// </summary>
        /// <param name="value">System as string.</param>
        public static implicit operator Source(string value) => new Source { Value = value };
    }
}