// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents an implementation of <see cref="IVerifyContractsCompatibility"/>.
/// </summary>
public class VerifyContractsCompatibility : IVerifyContractsCompatibility
{
    /// <inheritdoc />
    public bool IsCompatible(Version runtimeContractsVersion, Version headContractsVersion)
        => HasSameMajor(runtimeContractsVersion, headContractsVersion)
            && HasCompatibleMinor(runtimeContractsVersion, headContractsVersion);

    static bool HasSameMajor(Version version, Version otherVersion)
        => version.Major == otherVersion.Major;
    
    static bool HasCompatibleMinor(Version runtimeContractsVersion, Version headContractsVersion)
        => runtimeContractsVersion.Minor >= headContractsVersion.Minor;
}
