// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Defines the results of verifying compatibility of a Client SDK and a Runtime version.
/// </summary>
public enum ContractsCompatibility
{
    /// <summary>
    /// The value returned when the versions are compatible.
    /// </summary>
    Compatible = 0,
    
    /// <summary>
    /// The value returned when the Contracts version used by the Client SDK is older than required by the current Runtime version.
    /// </summary>
    ClientTooOld = 1,
    
    /// <summary>
    /// The value returned when the Contracts version used by the Client SDK is newer than supported by the current Runtime version.
    /// </summary>
    RuntimeTooOld = 2,
}
