// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents the number of handshake request attempts that have been made by a Client.
/// </summary>
/// <param name="Attempt">The attempt number.</param>
public record HandshakeAttempt(uint Attempt) : ConceptAs<uint>(Attempt)
{
    /// <summary>
    /// Implicitly convert from a <see cref="uint"/> to an <see cref="HandshakeAttempt"/>.
    /// </summary>
    /// <param name="attempt">HandshakeAttempt as a <see cref="uint"/>.</param>
    /// <returns>The <see cref="HandshakeAttempt"/> concept value.</returns>
    public static implicit operator HandshakeAttempt(uint attempt) => new(attempt);
}
