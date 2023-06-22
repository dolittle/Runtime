// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Defines a system that can parse handshake requests.
/// </summary>
public interface IRequestConverter
{
    /// <summary>
    /// Attempts to convert a received handshake request to the Runtime representation.
    /// </summary>
    /// <param name="request">The received handshake request.</param>
    /// <param name="parsed">The parsed handshake request, if parsing succeeded.</param>
    /// <param name="failure">The failure, if the parsing failed.</param>
    /// <returns>True if the parsing succeeded, false if not.</returns>
    bool TryConvert(HandshakeRequest request, [NotNullWhen(true)] out Request? parsed, [NotNullWhen(false)] out Failure? failure);
}
