// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents the identifier of a Dolittle client SDK.
/// </summary>
/// <param name="Identifier">The SDK identifier.</param>
public record SDKIdentifier(string Identifier) : ConceptAs<string>(Identifier)
{
    /// <summary>
    /// Implicitly convert a <see cref="string"/> to an <see cref="SDKIdentifier"/>.
    /// </summary>
    /// <param name="identifier">SDK identifier as <see cref="string"/>.</param>
    /// <returns>The <see cref="SDKIdentifier"/> concept value.</returns>
    public static implicit operator SDKIdentifier(string identifier) => new(identifier);
}
