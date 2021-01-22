// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Represents the failure id.
    /// </summary>
    public class FailureId : ConceptAs<Guid>
    {
        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents an undocumented error type.
        /// </summary>
        public static FailureId Other => Guid.Parse("05cc1d10-4efc-457b-84a6-a1be0a5f36ba");

        /// <summary>
        /// Implicitly converts the <see cref="Guid" /> to <see cref="FailureId" />.
        /// </summary>
        /// <param name="id"><see cref="Guid" /> to convert.</param>
        public static implicit operator FailureId(Guid id) => new FailureId { Value = id };

        /// <summary>
        /// Create a <see cref="FailureId" /> from a<see cref="string" /> representing a <see cref="Guid" /> .
        /// </summary>
        /// <param name="guidString">The <see cref="Guid" /> <see cref="string" />.</param>
        /// <returns><see cref="FailureId" />.</returns>
        public static FailureId Create(string guidString) => Guid.Parse(guidString);
    }
}
