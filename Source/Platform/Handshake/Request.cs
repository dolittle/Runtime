// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Client;
using Dolittle.Runtime.Domain.Platform;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents a handshake request received from a Client.
/// </summary>
/// <param name="SDK">The SDK identifier.</param>
/// <param name="SDKVersion">The SDK version.</param>
/// <param name="HeadVersion">The Head version.</param>
/// <param name="ContractsVersion">The version of the Contracts used by the Client SDK.</param>
/// <param name="Attempt">The handshake attempt number.</param>
/// <param name="TimeSpent">Time since the first handshake attempt.</param>
/// <param name="BuildResults">The build results.</param>
public record Request(
    SDKIdentifier SDK,
    Version SDKVersion,
    Version HeadVersion,
    Version ContractsVersion,
    HandshakeAttempt Attempt,
    HandshakeTimeSpent TimeSpent,
    BuildResults BuildResults);
