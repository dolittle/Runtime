// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an unique identifier (correlation id) for service requests.
    /// </summary>
    public record RequestId(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Generates a new <see cref="RequestId"/>.
        /// </summary>
        /// <returns>A <see cref="Guid"/> based request id.</returns>
        public static RequestId Generate() => Guid.NewGuid();

        /// <summary>
        /// Implicitly convert a <see cref="string" /> to a <see cref="RequestId" />.
        /// </summary>
        /// <param name="requestId">The request id.</param>
        public static implicit operator RequestId(string requestId) => new(requestId);

        /// <summary>
        /// Implicitly convert a <see cref="Guid" /> to a <see cref="RequestId" />.
        /// </summary>
        /// <param name="requestId">The request id.</param>
        public static implicit operator RequestId(Guid requestId) => new(requestId.ToString());

        /// <summary>
        /// Implicitly convert a <see cref="RequestId" /> to a <see cref="string" />.
        /// </summary>
        /// <param name="requestId">The request id.</param>
        public static implicit operator string(RequestId requestId) => requestId.Value;
    }
}
