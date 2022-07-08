// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Handshake.Contracts;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Holds the unique <see cref="FailureId">failure ids</see> used in handshakes.
/// </summary>
public static class HandshakeFailures
{
    /// <summary>
    /// Gets the <see cref="FailureId"/> that is returned when a <see cref="HandshakeRequest"/> is missing required information.
    /// </summary>
    public static FailureId MissingHandshakeInformation => FailureId.Create("8f5ba2c2-6543-47ce-b2a8-d92a872e7280");
    
    /// <summary>
    /// Gets the <see cref="FailureId"/> that is returned when the Contracts version of the Client SDK is too old to be used with the current Runtime.
    /// </summary>
    public static FailureId SDKMustBeUpdated => FailureId.Create("3e2d8368-4bca-4e0b-a9bf-8d1cccbcac21");
    
    /// <summary>
    /// Gets the <see cref="FailureId"/> that is returned when the Contracts version of the Runtime is too old to be used with the Client SDK.
    /// </summary>
    public static FailureId RuntimeMustBeUpdated => FailureId.Create("b46fb8a0-4565-4f00-880c-022cbb3bbabb");
}
