// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Platform;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Defines a system that verifies that the contracts versions of the Head and Runtime are compatible.
/// </summary>
public interface IVerifyContractsCompatibility
{
    /// <summary>
    /// Verifies whether the versions are compatible with each other.
    /// </summary>
    /// <param name="runtimeContractsVersion">The <see cref="Version"/> of the contracts for the Runtime.</param>
    /// <param name="headContractsVersion">The <see cref="Version"/> of the contracts for the Head.</param>
    /// <returns>A <see cref="ContractsCompatibility"/> result indicating whether or not the versions are compatible.</returns>
    ContractsCompatibility CheckCompatibility(Version runtimeContractsVersion, Version headContractsVersion);
}
