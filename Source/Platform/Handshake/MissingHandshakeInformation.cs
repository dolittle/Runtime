// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// The failure that is returned when a <see cref="HandshakeRequest"/> is missing information.
/// </summary>
/// <param name="Field">The field that is missing.</param>
public record MissingHandshakeInformation(string Field) : Failure(
    HandshakeFailures.MissingHandshakeInformation, 
    $"The received handshake is missing the field {Field}");
